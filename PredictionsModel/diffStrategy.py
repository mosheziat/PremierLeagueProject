import pandas as pd
import numpy as np
from random import randint
import random
from sklearn.utils import shuffle
from sklearn.ensemble import RandomForestClassifier
from sklearn.linear_model import LogisticRegression
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import LabelEncoder
from sklearn.metrics import accuracy_score
from sklearn.svm import SVC
from sklearn.multiclass import OneVsRestClassifier
from sklearn.ensemble import GradientBoostingClassifier
from sklearn.svm import SVC
from sklearn.multiclass import OneVsRestClassifier
import csv

very_important_features = ['AwayTeamOdds','DrawOdds', 'HomeTeamOdds', 'isHomeFavorite', 'HomeGoalsExpectedVsTeam',	'AwayGoalsExpectedVsTeam', 'HomeShotsTargetVsTeam',	'AwayShotsTargetVsTeam', 'AwayShotsTarget', 'HomeShotsTarget', 'HomePossession', 'AwayPossession', 'HomeTotalShotsVsTeam', 'AwayTotalShotsVsTeam']
very_important_features2 = ['AwayTeamOdds','DrawOdds', 'HomeTeamOdds', 'isHomeFavorite','AwayValue', 'HomeValue', 'HomeShotsTargetVsTeam',	'AwayShotsTargetVsTeam', 'AwayShotsTarget', 'HomeShotsTarget', 'HomePossession', 'AwayPossession', 'HomeTotalShotsVsTeam', 'AwayTotalShotsVsTeam']
very_important_features_lr = ['isHomeFavorite', 'HomeShotsTargetVsTeam',	'AwayShotsTargetVsTeam', 'AwayShotsTarget', 'AwayValue', 'HomeValue', 'HomeShotsTarget', 'HomePossession', 'AwayPossession', 'HomeTotalShotsVsTeam', 'AwayTotalShotsVsTeam']
only_odds = ['AwayTeamOdds','DrawOdds', 'HomeTeamOdds']
all_features = [ 'AwayTeamOdds', 'DrawOdds', 'HomeTeamOdds', 'HomeValue','AwayValue','HomePtsSeas','AwayPtsSeas','HomePtsLast','AwayPtsLast','HomePtsHome','AwayPtsAway','HomeGoalsSeas','AwayGoalsSeas','HomeGoalsLast','AwayGoalsLast','HomeGoalsHome','AwayGoalsAway','HomeGoalsConcSeas','AwayGoalsConcSeas','HomeGoalsConcLast','AwayGoalsConcLast','HomeGoalsConcHome','AwayGoalsConcAway','HomeTotalShots','AwayTotalShots','HomeShotsTarget','AwayShotsTarget','HomeTotalShotsAgst','AwayTotalShotsAgst','HomeShotsTargetAgst','AwayShotsTargetAgst', 'HomeTotalShotsVsTeam',	'AwayTotalShotsVsTeam',	'HomeShotsTargetVsTeam',	'AwayShotsTargetVsTeam','HomePossession','AwayPossession','HomeExpectedWin','AwayExpectedWin', 'HomeTrend', 'AwayTrend', 'HomeWeightedPoints',	'AwayWeightedPoints',	'ChanceCreationHome'	,'ChanceCreationAway',	'ShotsAccuracyHome',	'ShotsAccuracyAway',	'ScoringRateHome',	'ScoringRateAway',	'KeeperStrengthHome',	'KeeperStrengthAway', 'Round', 'HomeGoalsExpected',	'AwayGoalsExpected', 'HomeGoalsExpectedConc',	'AwayGoalsExpectedConc',	'HomeGoalsExpectedVsTeam',	'AwayGoalsExpectedVsTeam', 'isHomeFavorite']

def calculate_results_for_diffs_only(y_proba, y_test, odds_values):
    correct = 0
    all = 0
    correct_ratios = []
    if (len(y_proba) != len(y_test) or len(y_test) != len(odds_values)):
        return 0.0
    for i in range(len(y_proba)):
        relevant_proba = y_proba[i]
        relevant_odds = odds_values.iloc[i]
        correct_pred = y_test[i]
        max_proba = max(relevant_proba)
        max_index = relevant_proba.tolist().index(max_proba)
        relevant_ratio = relevant_odds[max_index]
        ratio = relevant_ratio * 0.9
        ratio = max(1.1, ratio)
        model_ratio = 1/max_proba
        if ratio > model_ratio:
            all = all + 1
            if max_index == correct_pred:
                correct = correct + 1
                correct_ratios.append(ratio)
    ratio_percent = 0
    if len(correct_ratios) > 0:
        ratio_percent = sum(correct_ratios) / len(correct_ratios)
    precision = 0
    if all > 0:
        precision = correct / all
    recall = all / len(y_test)
    #print (precision, recall, ratio_percent)
    return precision, recall, ratio_percent

def calculate_balance(recall, precision, ratios):
    participated_matches = recall * num_of_matches
    print("Participated:",participated_matches)
    correct_matches = participated_matches * precision
    print("Correct:",correct_matches)
    won = ratios * correct_matches
    print("Won:",won)
    return won - participated_matches

def get_result_for_model(train, labels, clf, file_path):
    r = randint(0, 1000)
    x_train, x_test, y_train, y_test = train_test_split(train, labels, test_size=0.2, random_state=r)
    odds_values = x_test[x_test.columns[0:3]]
    x_train = x_train[x_train.columns[3:]]
    x_test = x_test[x_test.columns[3:]]
    clf.fit(x_train, y_train)
    y_proba = clf.predict_proba(x_test)
    if file_path != "":
        write_results(y_proba, y_test, file_path)
    res = calculate_results_for_diffs_only(y_proba, y_test, odds_values)
    return res


def get_best_config():
    best_result = 0
    best_depth = 0
    best_estimators = 0
    best_features = []
    for x in range (1):
        #feats = peek_feature_groups_randomly(all_groups_of_features)
        #feats = peek_random_features(features)
        feats = very_important_features
        #feats = all_features
        #feats = only_odds
        train = input[feats]
        for i in range(1, 6):
            depth = i * 2
            for j in range(1, 11):
                estimators = j *200
                print (depth, estimators, feats)
                clf = RandomForestClassifier(max_depth=depth, n_estimators=estimators, bootstrap = False)
                res = run_experiment(train, y, clf)
                if res > best_result:
                    best_result = res
                    best_depth = depth
                    best_estimators = estimators
                    best_features = feats
                    print("********",best_result, "********")
    print(best_result, best_estimators, best_depth, best_features)

def run_experiment(features, labels, clf):
    results = []
    used_data = []
    correct_ratios = []
    for i in range(100):
        #print(i)
        result = get_result_for_model(features, labels, clf, "")
        results.append(result[0])
        used_data.append(result[1])
        correct_ratios.append(result[2])
    #print(sum(results) / len(results), np.std(results), sum(used_data) / len(used_data))
    recall = sum(used_data) / len(used_data)
    precision = sum(results) / len(results)
    ratios = sum(correct_ratios) / len(correct_ratios)
    balance = calculate_balance(recall, precision, ratios)
    balance_percent = balance / (recall * num_of_matches)
    print("Ratios: ", ratios)
    print("Recall: ", recall)
    print("Precision: ",  precision)
    print("Balance: ",  balance)
    print ("Balance Percent Change: ", balance_percent)
    f_score = (2 * precision * recall) / (precision + recall)
    print("F-Measure: ", f_score)
    return balance_percent


input = pd.read_csv("full_data2.csv")
input = shuffle(input)
print(input.shape)
start_round = min(input['Round'])
last_round = max(input['Round'])

num_of_matches = (last_round - start_round) * 1000
print("Number of Matches:", num_of_matches)
lb = LabelEncoder()
target = input["Winner"]
y = lb.fit_transform(target)

get_best_config()