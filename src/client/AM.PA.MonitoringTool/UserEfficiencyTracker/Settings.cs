﻿// Created by André Meyer (ameyer@ifi.uzh.ch) from the University of Zurich
// Created: 2015-10-20
// 
// Licensed under the MIT License.

using System;
using Shared.Data;

namespace UserEfficiencyTracker
{
    public static class Settings
    {
        public static bool DefaultPopUpIsEnabled = true;
        public const int DefaultPopUpInterval = 60; // in minutes

        //public static bool IsEnabled = MiniSurveysEnabled;
        public const string DbTableIntervalPopup = "user_efficiency_survey";
        public const string DbTableDailyPopUp = "user_efficiency_survey_day";

        private const double IntervalPostponeShortInMinutes = 5.0; // every 5mins
        private const double SurveyCheckerMinutes = 1.0; // every minute
        private const double IntervalCloseIfNotAnsweredAfterHours = 2.0; // close survey if not answered after 2 hours

        public static TimeSpan IntervalPostponeShortInterval = TimeSpan.FromMinutes(IntervalPostponeShortInMinutes);
        public static TimeSpan SurveyCheckerInterval = TimeSpan.FromMinutes(SurveyCheckerMinutes);
        public static TimeSpan IntervalCloseIfNotAnsweredInterval = TimeSpan.FromHours(IntervalCloseIfNotAnsweredAfterHours);
        public static TimeSpan DailyPopUpEarliestMoment = new TimeSpan(4, 0, 0); // 4 am in the morning
    }
}