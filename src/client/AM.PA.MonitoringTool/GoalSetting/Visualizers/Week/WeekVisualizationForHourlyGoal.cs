﻿// Created by Sebastian Mueller (smueller@ifi.uzh.ch) from the University of Zurich
// Created: 2017-04-13
// 
// Licensed under the MIT License.

using System;
using GoalSetting.Goals;
using GoalSetting.Model;
using Shared;
using Shared.Helpers;
using GoalSetting.Data;
using System.Linq;

namespace GoalSetting.Visualizers.Week
{
    public class WeekVisualizationForHourlyGoal : PAVisualization
    {
        public WeekVisualizationForHourlyGoal(DateTimeOffset date, GoalActivity goal) : base(date, goal) { }

        public override string GetHtml()
        {
            var html = string.Empty;
            
            // CSS
            html += "<style type='text/css'>";
            html += ".c3-line { stroke-width: 2px; }";
            html += ".c3-grid text, c3.grid line { fill: black; }";
            html += ".axis path, .axis line {fill: none; stroke: black; stroke-width: 1; shape-rendering: crispEdges;}";
            html += "</style>";

            //HTML
            html += "<div id='" + VisHelper.CreateChartHtmlTitle(Title) + "' style='align: center'></div>";
            html += "<p style='text-align: center; font-size: 0.66em;'>" + GoalVisHelper.GetHintText(_goal, VisType.Week) + "</p>";

            //JS
            html += "<script>";

            html += "var actualHeight = document.getElementsByClassName('item Wide')[0].offsetHeight;";
            html += "var actualWidth = document.getElementsByClassName('item Wide')[0].offsetWidth;";
            html += "var margin = {top: 10, right: 30, bottom: 30, left: 30}, width = (actualWidth * 0.97)- margin.left - margin.right, height = (actualHeight * 0.73) - margin.top - margin.bottom;";
            
            html += GenerateData();
            html += "console.log(gridData);";

            html += "var grid = d3.select('#" + VisHelper.CreateChartHtmlTitle(Title) + "').append('svg')";
            html += @".attr('width', width + margin.left + margin.right).attr('height', height + margin.top + margin.bottom)
                    .append('g').attr('transform', 'translate(' + margin.left + ',' + margin.top + ')');";

            html += "var row = grid.selectAll('.row')";
            html += ".data(gridData)";
            html += ".enter().append('g')";
            html += ".attr('class', 'row');";

            html += "var column = row.selectAll('.square')";
            html += ".data(function(d) { return d; })";
            html += ".enter().append('rect')";
            html += ".attr('class', 'square')";
            html += ".attr('x', function(d) { return d.x * (width / 25); })";
            html += ".attr('y', function(d) { return d.y * (height / 8); })";
            html += ".attr('width', (width / 25))";
            html += ".attr('height', (height / 8))";
            html += @".style('fill', function(d) {
                if (d.type === 'Title') {
                    return '#999';
                } else if (d.hasValue === 'False') {
                    return '#fff';
                } else {
                    if (d.success === 'True') {
                        return '#0F0';
                    }
                    return '#F00';
                }
                return '#fff';})";
            html += ".style('stroke', '#222');";

            html += "var text = row.selectAll('.label')";
            html += ".data(function(d) { return d; })";
            html += ".enter().append('svg:text')";
            html += ".attr('x', function(d) { return (d.x * (width / 25)) + ((width / 25) / 2); })";
            html += ".attr('y', function(d) { return (d.y * (height / 8)) + ((height / 8) / 2); })";
            html += ".attr('text-anchor', 'middle')";
            html += ".attr('dy', '.35em')";
            html += ".text(function(d) { return d.value });";

            html += "</script>";

            return html;
        }

        private string GenerateData()
        {
            var html = string.Empty;

            html += "var gridData = [";

            double xPos = 1;
            double yPos = 1;
            string newValue = "";
            string newType = "";
            bool success = false;
            bool hasValue = false;

            for (var row = 0; row < 8; row++)
            {
                html += "[";
                
                for (var column = 0; column < 25; column++)
                {
                    if (row == 0)
                    {
                        if (column != 0)
                        {
                            newValue = "" + (column - 1);
                        }
                    }
                    else if (column == 0)
                    {
                        if (row == 1)
                        {
                            newValue = "Mo";
                        }
                        if (row == 2)
                        {
                            newValue = "Di";
                        }
                        if (row == 3)
                        {
                            newValue = "Mi";
                        }
                        if (row == 4)
                        {
                            newValue = "Do";
                        }
                        if (row == 5)
                        {
                            newValue = "Fr";
                        }
                        if (row == 6)
                        {
                            newValue = "Sa";
                        }
                        if (row == 7)
                        {
                            newValue = "So";
                        }
                    }
                    else
                    {
                        var firstDayOfWeek = DateTimeHelper.GetFirstDayOfWeek_Iso8801(base._date);
                        firstDayOfWeek = new DateTime(firstDayOfWeek.Year, firstDayOfWeek.Month, firstDayOfWeek.Day, column - 1, 0, 0, 0);
                        var dateToCheck = firstDayOfWeek.AddDays(row - 1);
                        var dateToCheckEnd = dateToCheck.AddMinutes(59);
                        dateToCheckEnd = dateToCheckEnd.AddSeconds(59);

                        if (dateToCheck > DateTime.Now)
                        {
                            newValue = "";
                            hasValue = false;
                        }
                        else
                        {
                            Tuple<string, bool> values = GetValue(dateToCheck, dateToCheckEnd);
                            newValue = values.Item1;
                            success = values.Item2;
                            hasValue = true;
                        }
                    }

                    if (row == 0 || column == 0)
                    {
                        newType = "Title";
                    }
                    else
                    {
                        newType = "Value";
                    }

                    html += "{type:'" + newType + "', x:" + xPos + ", y:" + yPos + ", value:'" + newValue + "', success: '" + success + "', hasValue: '" + hasValue + "'},";
                    xPos += 1;

                }
                html = html.Remove(html.Length - 1);
                html += "],";
                xPos = 1;
                yPos += 1;
            }

            html = html.Remove(html.Length - 1);
            html += "];";
            
            return html;
        }

        private Tuple<string, bool> GetValue(DateTimeOffset start, DateTimeOffset end)
        {
            var activities = DatabaseConnector.GetActivitiesSinceAndBefore(start.DateTime, end.DateTime);
            activities = DataHelper.MergeSameActivities(activities, Settings.MinimumSwitchTimeInSeconds);
            activities = activities.Where(a => a.Activity.Equals(base._goal.Activity)).ToList();
            
            if (activities.Count < 1)
            {
                return Tuple.Create<string, bool>("", false);
            }

            switch (base._goal.Rule.Goal)
            {
                case RuleGoal.NumberOfSwitchesTo:
                    int numberOfSwitches = DataHelper.GetNumberOfSwitchesToActivity(activities, base._goal.Activity);
                    return Tuple.Create<string, bool>("" + numberOfSwitches, DataHelper.SuccessRule(base._goal.Rule, numberOfSwitches));
                case RuleGoal.TimeSpentOn:
                    double timeSpentOn = DataHelper.GetTotalTimeSpentOnActivity(activities, base._goal.Activity).TotalMilliseconds;
                    return Tuple.Create<string, bool>(DataHelper.GetTotalTimeSpentOnActivity(activities, base._goal.Activity).TotalMinutes.ToString("N0"), DataHelper.SuccessRule(base._goal.Rule, timeSpentOn));
            }
            return Tuple.Create<string, bool>("", false);
        }
    }
}