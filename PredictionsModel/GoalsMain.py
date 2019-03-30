import pandas as pd
import numpy as np
from random import randint
import random
from sklearn.utils import shuffle
from sklearn.ensemble import RandomForestClassifier
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import LabelEncoder
import csv

round = ['Round']
is_favorite = ['IsHomeFavorite']
value_features = ['HomeValue','AwayValue']
trend = ['HomeTrend', 'AwayTrend']
odds_ratio = ['HomeTeamOdds',	'DrawOdds',	'AwayTeamOdds']
raw_stats_vs_team = ['HomeTotalShotsVsTeam',	'AwayTotalShotsVsTeam',	'HomeShotsTargetVsTeam',	'AwayShotsTargetVsTeam']
expected_goals = ['HomeGoalsExpected',	'AwayGoalsExpected', 'HomeGoalsExpectedConc',	'AwayGoalsExpectedConc']
expected_goals_vs_team = ['HomeGoalsExpectedVsTeam',	'AwayGoalsExpectedVsTeam']
seasonal_pts = ['HomePtsSeas','AwayPtsSeas']
seasonal_goals = ['HomeGoalsSeas','AwayGoalsSeas','HomeGoalsConcSeas','AwayGoalsConcSeas']
weighted_features = ['HomeWeightedPoints',	'AwayWeightedPoints']
hoa_goals = ['HomeGoalsHome','AwayGoalsAway','HomeGoalsConcHome','AwayGoalsConcAway']
hoa_pts = ['HomePtsHome','AwayPtsAway']
last_games_goals =['HomeGoalsLast','AwayGoalsLast','HomeGoalsConcLast','AwayGoalsConcLast']
last_games_pts = ['HomePtsLast','AwayPtsLast']
raw_stats = ['HomeTotalShots','AwayTotalShots','HomeShotsTarget','AwayShotsTarget','HomePossession','AwayPossession']
raw_stats_agst = ['HomeTotalShotsAgst','AwayTotalShotsAgst','HomeShotsTargetAgst','AwayShotsTargetAgst']
shots_vs_team = ['HomeTotalShotsVsTeam',	'AwayTotalShotsVsTeam',	'HomeShotsTargetVsTeam',	'AwayShotsTargetVsTeam']
abstracted_stats = ['ChanceCreationHome','ChanceCreationAway','ShotsAccuracyHome','ShotsAccuracyAway','ScoringRateHome','ScoringRateAway','KeeperStrengthHome','KeeperStrengthAway']
all_groups_of_features = [is_favorite,odds_ratio, value_features, round, trend, raw_stats_vs_team, expected_goals, expected_goals_vs_team, seasonal_pts, seasonal_goals, weighted_features, hoa_pts, hoa_goals, last_games_pts, last_games_goals, raw_stats, raw_stats_agst, shots_vs_team, abstracted_stats, is_favorite]
features_includes_competition = [ 'AwayTeamOdds', 'DrawOdds', 'HomeTeamOdds', 'CompetitionID', 'Over2.5', 'HomeValue','AwayValue', 'HomePtsSeas','AwayPtsSeas','HomePtsLast','AwayPtsLast','HomePtsHome','AwayPtsAway','HomeGoalsSeas','AwayGoalsSeas','HomeGoalsLast','AwayGoalsLast','HomeGoalsHome','AwayGoalsAway','HomeGoalsConcSeas','AwayGoalsConcSeas','HomeGoalsConcLast','AwayGoalsConcLast','HomeGoalsConcHome','AwayGoalsConcAway','HomeTotalShots','AwayTotalShots','HomeShotsTarget','AwayShotsTarget','HomeTotalShotsAgst','AwayTotalShotsAgst','HomeShotsTargetAgst','AwayShotsTargetAgst', 'HomeTotalShotsVsTeam',	'AwayTotalShotsVsTeam',	'HomeShotsTargetVsTeam',	'AwayShotsTargetVsTeam','HomePossession','AwayPossession', 'HomeTrend', 'AwayTrend', 'HomeWeightedPoints',	'AwayWeightedPoints',	'ChanceCreationHome'	,'ChanceCreationAway',	'ShotsAccuracyHome',	'ShotsAccuracyAway',	'ScoringRateHome',	'ScoringRateAway',	'KeeperStrengthHome',	'KeeperStrengthAway',  'HomeGoalsExpected',	'AwayGoalsExpected', 'HomeGoalsExpectedConc',	'AwayGoalsExpectedConc',	'HomeGoalsExpectedVsTeam',	'AwayGoalsExpectedVsTeam', 'IsHomeFavorite']
goals_features_includes_competition = [ 'AwayTeamOdds', 'DrawOdds', 'HomeTeamOdds', 'CompetitionID', 'Over2.5','AwayTeamOdds', 'DrawOdds', 'HomeTeamOdds', 'HomeValue','AwayValue','HomeGoalsSeas','AwayGoalsSeas','HomeGoalsLast','AwayGoalsLast','HomeGoalsHome','AwayGoalsAway','HomeGoalsConcSeas','AwayGoalsConcSeas','HomeGoalsConcLast','AwayGoalsConcLast','HomeGoalsConcHome','AwayGoalsConcAway','HomeTotalShots','AwayTotalShots','HomeShotsTarget','AwayShotsTarget','HomeTotalShotsAgst','AwayTotalShotsAgst','HomeShotsTargetAgst','AwayShotsTargetAgst', 'HomeTotalShotsVsTeam','AwayTotalShotsVsTeam',	'HomeShotsTargetVsTeam',	'AwayShotsTargetVsTeam','HomePossession','AwayPossession',	'ChanceCreationHome'	,'ChanceCreationAway',	'ShotsAccuracyHome',	'ShotsAccuracyAway',	'ScoringRateHome',	'ScoringRateAway',	'KeeperStrengthHome',	'KeeperStrengthAway',  'HomeGoalsExpected',	'AwayGoalsExpected', 'HomeGoalsExpectedConc',	'AwayGoalsExpectedConc',	'HomeGoalsExpectedVsTeam',	'AwayGoalsExpectedVsTeam', 'IsHomeFavorite']


def peek_random_features(features):
    num_of_features = randint(2, len(features) - 1)
    picked_groups = random.sample(range(1, len(features)), num_of_features)
    group = []
    for i in picked_groups:
        group.append(features[i])
    return group

def peek_feature_groups_randomly(all_groups):
    num_of_features = randint(1, len(all_groups) - 1)
    picked_groups = random.sample(range(1, len(all_groups)), num_of_features)
    group = []
    for i in picked_groups:
        group += all_groups[i]
    return group


def get_best_config():
    best_result = 0
    best_depth = 0
    best_estimators = 0
    best_features = []
    for x in range (500):
        #feats = peek_feature_groups_randomly(all_groups_of_features)
        #feats = peek_random_features(features)
        #feats = goals_features_includes_competition
        basic_feats = ['AwayTeamOdds', 'DrawOdds', 'HomeTeamOdds', 'CompetitionID', 'Over2.5']
        new_feats = peek_feature_groups_randomly(all_groups_of_features)
        print(new_feats)
        feats = basic_feats + new_feats
        train = input[feats]
        for i in range(1, 2):
            depth = i * 2
            for j in range(1, 2):
                estimators = j *50
                print (depth, estimators, feats)
                clf = RandomForestClassifier(max_depth=depth, n_estimators=estimators, bootstrap = False)
                res = run_experiment(train, clf)
                if res > best_result:
                    best_result = res
                    best_depth = depth
                    best_estimators = estimators
                    best_features = feats
                    print(best_result)
    print(best_result, best_estimators, best_depth, best_features)

def run_experiment(features, clf):
    results = []
    used_data = []
    for i in range(50):
        #print(i)
        result = get_result_for_model(features, clf, "")
        results.append(result[0])
        used_data.append(result[1])
    #print(sum(results) / len(results), np.std(results), sum(used_data) / len(used_data))
    return sum(results) / len(results)

def get_result_for_model(train, clf, file_path):

    lb = LabelEncoder()

    past_seasons = train.loc[train['CompetitionID'] < 8]
    last_season = train.loc[train['CompetitionID'] == 8]
    x_train = past_seasons[past_seasons.columns[5:]]
    target_train = past_seasons["Over2.5"]
    y_train = lb.fit_transform(target_train)

    target_test = last_season["Over2.5"]
    y_test = lb.fit_transform(target_test)
    x_test = last_season[last_season.columns[5:]]
    clf.fit(x_train, y_train)
    y_proba = clf.predict_proba(x_test)


    #r = randint(0, 1000)
    #x_train, x_test, y_train, y_test = train_test_split(train, labels, test_size=0.2, random_state=r)
    #clf.fit(x_train, y_train)
    #y_pred = clf.predict(x_test)
    #y_proba = clf.predict_proba(x_test)
    if file_path != "":
        write_results(y_proba, y_test, file_path)
    #res =  accuracy_score(y_test, y_pred)
    res = get_results_for_threshold(y_proba, y_test)
    return res

def get_results_for_threshold(y_proba, y_test):
    correct = 0
    all = 0
    #print(y_test[0])
    if (len(y_proba) != len(y_test)):
        return 0.0
    for i in range(len(y_proba)):
        relevant_proba = y_proba[i]
        correct_pred = y_test[i]
        max_proba = max(relevant_proba)
        max_index = relevant_proba.tolist().index(max_proba)
        #if max_index == 1:
        #    continue
        all = all + 1
        if max_index == correct_pred:
            correct = correct + 1
    return correct / all, all/len(y_test)


input = pd.read_csv("full_data_4.csv")
input = shuffle(input)
#input = input.loc[input['Round'] > 0.07]
#lb = LabelEncoder()
#target = input["Over2.5"]
#y = lb.fit_transform(target)

get_best_config()