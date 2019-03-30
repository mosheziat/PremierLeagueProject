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



def design_equaly_devided_set():
    draw = input.loc[input['Winner'] == 'D']
    away = input.loc[input['Winner'] == 'A'][:len(draw)]
    home = input.loc[input['Winner'] == 'H'][:len(draw)]

    designed = draw
    designed = designed.append(home)
    designed = designed.append(away)
    return designed

important_features_include_competition = ['AwayTeamOdds', 'DrawOdds', 'HomeTeamOdds', 'CompetitionID', 'Winner', 'ExpectedGoalsDiff', 'ShotsTargetDiff', 'HomeShotsTargetVsTeam',	'AwayShotsTargetVsTeam', 'AwayShotsTarget',  'HomeShotsTarget', 'HomePossession', 'AwayPossession', 'HomeTotalShotsVsTeam', 'AwayTotalShotsVsTeam', 'AwayTotalShotsAgst', 'AwayWeightedPoints', 'HomeWeightedPoints', 'HomeGoalsExpectedVsTeam', 'AwayGoalsExpectedVsTeam', 'HomeGoalsSeas', 'AwayGoalsSeas', 'HomeTotalShots', 'AwayTotalShots', 'AwayGoalsExpected', 'HomeGoalsExpected', 'AwayGoalsExpectedConc', 'HomeGoalsExpectedConc', 'HomePtsSeas', 'AwayPtsSeas']
very_important_features_include_competition = ['AwayTeamOdds','DrawOdds', 'HomeTeamOdds', 'CompetitionID', 'Winner', 'isHomeFavorite', 'HomeGoalsExpectedVsTeam',	'AwayGoalsExpectedVsTeam', 'HomeShotsTargetVsTeam',	'AwayShotsTargetVsTeam', 'AwayShotsTarget', 'HomeShotsTarget', 'HomePossession', 'AwayPossession', 'HomeTotalShotsVsTeam', 'AwayTotalShotsVsTeam']

features_includes_competition = [ 'AwayTeamOdds', 'DrawOdds', 'HomeTeamOdds', 'CompetitionID', 'Winner', 'HomeValue','AwayValue', 'HomePtsSeas','AwayPtsSeas','HomePtsLast','AwayPtsLast','HomePtsHome','AwayPtsAway','HomeGoalsSeas','AwayGoalsSeas','HomeGoalsLast','AwayGoalsLast','HomeGoalsHome','AwayGoalsAway','HomeGoalsConcSeas','AwayGoalsConcSeas','HomeGoalsConcLast','AwayGoalsConcLast','HomeGoalsConcHome','AwayGoalsConcAway','HomeTotalShots','AwayTotalShots','HomeShotsTarget','AwayShotsTarget','HomeTotalShotsAgst','AwayTotalShotsAgst','HomeShotsTargetAgst','AwayShotsTargetAgst', 'HomeTotalShotsVsTeam',	'AwayTotalShotsVsTeam',	'HomeShotsTargetVsTeam',	'AwayShotsTargetVsTeam','HomePossession','AwayPossession', 'HomeTrend', 'AwayTrend', 'HomeWeightedPoints',	'AwayWeightedPoints',	'ChanceCreationHome'	,'ChanceCreationAway',	'ShotsAccuracyHome',	'ShotsAccuracyAway',	'ScoringRateHome',	'ScoringRateAway',	'KeeperStrengthHome',	'KeeperStrengthAway',  'HomeGoalsExpected',	'AwayGoalsExpected', 'HomeGoalsExpectedConc',	'AwayGoalsExpectedConc',	'HomeGoalsExpectedVsTeam',	'AwayGoalsExpectedVsTeam', 'IsHomeFavorite']
all_features = [ 'AwayTeamOdds', 'DrawOdds', 'HomeTeamOdds', 'HomeValue','AwayValue','HomePtsSeas','AwayPtsSeas','HomePtsLast','AwayPtsLast','HomePtsHome','AwayPtsAway','HomeGoalsSeas','AwayGoalsSeas','HomeGoalsLast','AwayGoalsLast','HomeGoalsHome','AwayGoalsAway','HomeGoalsConcSeas','AwayGoalsConcSeas','HomeGoalsConcLast','AwayGoalsConcLast','HomeGoalsConcHome','AwayGoalsConcAway','HomeTotalShots','AwayTotalShots','HomeShotsTarget','AwayShotsTarget','HomeTotalShotsAgst','AwayTotalShotsAgst','HomeShotsTargetAgst','AwayShotsTargetAgst', 'HomeTotalShotsVsTeam',	'AwayTotalShotsVsTeam',	'HomeShotsTargetVsTeam',	'AwayShotsTargetVsTeam','HomePossession','AwayPossession','HomeExpectedWin','AwayExpectedWin', 'HomeTrend', 'AwayTrend', 'HomeWeightedPoints',	'AwayWeightedPoints',	'ChanceCreationHome'	,'ChanceCreationAway',	'ShotsAccuracyHome',	'ShotsAccuracyAway',	'ScoringRateHome',	'ScoringRateAway',	'KeeperStrengthHome',	'KeeperStrengthAway', 'Round', 'HomeGoalsExpected',	'AwayGoalsExpected', 'HomeGoalsExpectedConc',	'AwayGoalsExpectedConc',	'HomeGoalsExpectedVsTeam',	'AwayGoalsExpectedVsTeam', 'isHomeFavorite']
value_features = ['HomeValue','AwayValue']
round = ['Round']
is_favorite = ['IsHomeFavorite']
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
#features_groups = [value_features + raw_stats, value_features + abstracted_stats, value_features + abstracted_stats + weighted_features, value_features + abstracted_stats + raw_stats, value_features + weighted_features + raw_stats, value_features + weighted_features+raw_stats + hoa_stats, raw_stats + weighted_features + last_games_stats]
#features_groups = [value_features + weighted_features + raw_stats + seasonal_features + hoa_stats + last_games_stats, value_features + weighted_features + expected_goals + expected_goals_vs_team + raw_stats + raw_stats_vs_team, weighted_features + abstracted_stats + raw_stats_vs_team + expected_goals_vs_team, weighted_features + abstracted_stats + raw_stats + expected_goals_vs_team + trend]
all_groups_of_features = [is_favorite, value_features, round, trend, raw_stats_vs_team, expected_goals, expected_goals_vs_team, seasonal_pts, seasonal_goals, weighted_features, hoa_pts, hoa_goals, last_games_pts, last_games_goals, raw_stats, raw_stats_agst, shots_vs_team, abstracted_stats, is_favorite]
important_features = ['AwayTeamOdds','DrawOdds', 'HomeTeamOdds', 'HomeShotsTargetVsTeam',	'AwayShotsTargetVsTeam', 'AwayShotsTarget',  'HomeShotsTarget', 'HomePossession', 'AwayPossession', 'HomeTotalShotsVsTeam', 'AwayTotalShotsVsTeam', 'HomeGoalsExpectedVsTeam', 'HomePtsSeas', 'HomeShotsTarget', 'HomeTotalShots', 'AwayWeightedPoints', 'AwayTotalShotsAgst', 'HomeWeightedPoints', 'HomeGoalsSeas', 'AwayGoalsExpectedVsTeam', 'AwayGoalsExpected', 'AwayGoalsSeas', 'AwayTotalShots', 'AwayGoalsExpectedConc']
very_important_features = ['AwayTeamOdds','DrawOdds', 'HomeTeamOdds', 'isHomeFavorite', 'HomeGoalsExpectedVsTeam',	'AwayGoalsExpectedVsTeam', 'HomeShotsTargetVsTeam',	'AwayShotsTargetVsTeam', 'AwayShotsTarget', 'HomeShotsTarget', 'HomePossession', 'AwayPossession', 'HomeTotalShotsVsTeam', 'AwayTotalShotsVsTeam']
very_important_features2 = ['AwayTeamOdds','DrawOdds', 'HomeTeamOdds', 'isHomeFavorite', 'HomeGoalsExpectedVsTeam',	'AwayGoalsExpectedVsTeam', 'HomeShotsTargetVsTeam',	'AwayShotsTargetVsTeam', 'AwayShotsTarget', 'HomeShotsTarget', 'HomePossession', 'AwayPossession', 'HomeTotalShotsVsTeam', 'AwayTotalShotsVsTeam']
very_important_features_lr = ['AwayTeamOdds','DrawOdds', 'HomeTeamOdds', 'isHomeFavorite', 'HomeWeightedPoints',	'AwayWeightedPoints', 'HomeShotsTargetVsTeam',	'AwayShotsTargetVsTeam', 'AwayShotsTarget', 'AwayValue', 'HomeValue', 'HomeShotsTarget', 'HomePossession', 'AwayPossession', 'HomeTotalShotsVsTeam', 'AwayTotalShotsVsTeam']
only_odds = ['AwayTeamOdds','DrawOdds', 'HomeTeamOdds']


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

def get_best_config_season_test(original_data):

    best_result = 0
    best_depth = 0
    best_estimators = 0
    thresholds = [0.30, 0.68]
    estimators_group = [5000, 2500]
    depths = [50]
    for x in range(1):
        basic_feats = ['AwayTeamOdds', 'DrawOdds', 'HomeTeamOdds', 'CompetitionID', 'Winner']
        #new_feats = peek_feature_groups_randomly(all_groups_of_features)
#       print(new_feats)
        #feats = basic_feats + new_feats
        for threshold in thresholds:
            feats = features_includes_competition
            # feats = very_important_features_include_competition
            # feats = important_features_include_competition
            data = original_data[feats]
            for depth in depths:
                for estimators in estimators_group:
                    print(depth, estimators, threshold, feats)
                    clf = RandomForestClassifier(max_depth=depth, n_estimators=estimators, bootstrap=False)
                    res = run_season_experiment(data, clf, threshold=threshold)
                    if res > best_result:
                        best_result = res
                        best_depth = depth
                        best_estimators = estimators
                        print("**************", best_result)
    print(best_result, best_estimators, best_depth)




def run_season_experiment(data, clf, threshold):
    results = []
    used_data = []
    correct_ratios = []
    for i in range(50):
        #print(i)
        result = run_season_test(data, clf, threshold, "")
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

def run_season_test(data, clf, threshold, file_path):
    lb = LabelEncoder()
    past_seasons = data.loc[data['CompetitionID'] < 8]
    last_season = data.loc[data['CompetitionID'] == 8]
    x_train = past_seasons[past_seasons.columns[5:]]
    target_train = past_seasons["Winner"]
    y_train = lb.fit_transform(target_train)

    target_test = last_season["Winner"]
    y_test = lb.fit_transform(target_test)
    x_test = last_season[last_season.columns[5:]]
    clf.fit(x_train, y_train)
    y_proba = clf.predict_proba(x_test)
    if file_path != "":
        write_results(y_proba, y_test, file_path)
    odds_values = last_season[last_season.columns[0:3]]
    #res = calculate_results_for_threshold(y_proba, y_test, threshold)
    res = calculate_results_for_threshold(y_proba, y_test, odds_values, threshold)
    return res

def get_best_logistic_config():
    best_result = 0
    best_threshold = 0
    for x in range (1):
        #feats = peek_feature_groups_randomly(all_groups_of_features)
        #feats = peek_random_features(all_features)
        #feats = very_important_features_lr
        feats = all_features
        train = input[feats]
        thresholds_to_check = [0.2, 0.35, 0.4, 0.45, 0.5, 0.52, 0.55, 0.57, 0.6, 0.62, 0.65]
        for threshold in thresholds_to_check:
            print(threshold, feats)
            clf = LogisticRegression()
            try:
                res = run_experiment(train, y, clf, threshold)
                if res > best_result:
                    best_result = res
                    best_threshold = threshold
                    print("********",best_result, "********")
            except:
                print("Exception!")
    print(best_result, best_threshold)

def get_best_config():
    best_result = 0
    best_depth = 0
    best_estimators = 0
    best_threshold = 0
    best_features = []
    for x in range (1):
        #feats = peek_feature_groups_randomly(all_groups_of_features)
        #feats = peek_random_features(features)
        #feats = very_important_features
        feats = all_features
        #feats = important_features
        #feats = only_odds
        train = input[feats]
        thresholds_to_check = [0.57, 0.62, 0.65]
        for threshold in thresholds_to_check:
            for i in range(3, 6):
                depth = i * 5
                for j in range(3, 6):
                    estimators = j *500
                    print (depth, estimators, threshold, feats)
                    clf = RandomForestClassifier(max_depth=depth, n_estimators=estimators, bootstrap = False)
                    res = run_experiment(train, y, clf, threshold)
                    if res > best_result:
                        best_result = res
                        best_depth = depth
                        best_estimators = estimators
                        best_threshold = threshold
                        best_features = feats
                        print("********",best_result, "********")
    print(best_result, best_estimators, best_depth, best_threshold, best_features)





def run_experiment(features, labels, clf, threhsold):
    results = []
    used_data = []
    correct_ratios = []
    for i in range(100):
        #print(i)
        result = get_result_for_model(features, labels, clf, threhsold, "")
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

def calculate_balance(recall, precision, ratios):
    participated_matches = recall * num_of_matches
    print("Participated:",participated_matches)
    correct_matches = participated_matches * precision
    print("Correct:",correct_matches)
    won = ratios * correct_matches
    print("Won:",won)
    return won - participated_matches







def get_result_for_model(train, labels, clf, threshold, file_path):
    r = randint(0, 1000)
    x_train, x_test, y_train, y_test = train_test_split(train, labels, test_size=0.2, random_state=r)
    odds_values = x_test[x_test.columns[0:3]]
    #without odds
    x_train = x_train[x_train.columns[3:]]
    x_test = x_test[x_test.columns[3:]]
    clf.fit(x_train, y_train)
    y_proba = clf.predict_proba(x_test)
    if file_path != "":
        write_results(y_proba, y_test, file_path)
    #res =  accuracy_score(y_test, y_pred)
    res = calculate_results_for_threshold(y_proba, y_test, odds_values, threshold)
    return res


def get_features_importance_for_model(train, labels, clf):
    r = randint(0, 1000)
    x_train, x_test, y_train, y_test = train_test_split(train, labels, test_size=0.2, random_state=r)
    clf.fit(x_train, y_train)
    feature_importances = pd.DataFrame(clf.feature_importances_,
                                       index=x_train.columns,
                                       columns=['importance']).sort_values('importance', ascending=False)
    return feature_importances

def run_feature_importance_experiment(train, labels, clf):
    #print(labels[0])
    importance = get_features_importance_for_model(train, labels, clf)
    for i in range(2, 500):
        cur_importance = get_features_importance_for_model(train, labels, clf)
        for key, value in cur_importance.items():
            importance[key] = (importance[key] + value)

    for key, value in importance.items():
        importance[key] = importance[key] / 500

    importance = importance.sort_values('importance', ascending=False)
    print(importance)
    return importance

def write_results(y_proba, y_test, file_path):
    if (len(y_proba) != len(y_test)):
        return
    with open(file_path, 'w', newline='') as csvfile:
        spamwriter =csv.writer(csvfile, delimiter=',', quoting=csv.QUOTE_MINIMAL)
        for i in range(len(y_test)):
            to_write = y_proba[i].tolist()
            max1 = max(y_proba[i])
            to_write.append(max1)
            max_index = y_proba[i].tolist().index(max(y_proba[i]))
            to_write.append(max_index)
            to_write.append(y_test[i])
            is_correct = 0
            if max_index == y_test[i]:
                is_correct = 1
            to_write.append(is_correct)
            spamwriter.writerow(to_write)







def calculate_results_for_threshold(y_proba, y_test, odds_values, threshold):
    correct = 0
    all = 0
    correct_ratios = []
    if (len(y_proba) != len(y_test) or len(y_test) != len(odds_values)):
        return 0.0
    for i in range(len(y_proba)):
        relevant_proba = y_proba[i]
        #print(relevant_proba)
        relevant_odds = odds_values.iloc[i]
        #print(relevant_odds)
        correct_pred = y_test[i]
        max_proba = max(relevant_proba)
        #print(max_proba)
        if max_proba < threshold:
            continue
        max_index = relevant_proba.tolist().index(max_proba)
        relevant_ratio = relevant_odds[max_index]
        #print(relevant_ratio)
        ratio = relevant_ratio  * 0.9
        ratio = max(1.1, ratio)
        all = all + 1
        #print(max_index, correct_pred)
        if max_index == correct_pred:
            correct = correct + 1
            correct_ratios.append(ratio)
    ratio_percent = 0
    if len(correct_ratios) > 0:
        ratio_percent = sum(correct_ratios) / len(correct_ratios)
    return correct / all, all/len(y_test), ratio_percent





input = pd.read_csv("full_data_4.csv")
input = shuffle(input)
print(input.shape)
#input = input.loc[input['Round'] > 0.09]
start_round = min(input['Round'])
last_round = max(input['Round'])

num_of_matches = (last_round - start_round) * 1000
print("Number of Matches:", num_of_matches)
lb = LabelEncoder()
target = input["Winner"]
y = lb.fit_transform(target)
#past_seasons = pd.read_csv("past_seasons.csv")
#last_season = pd.read_csv("last_season.csv")
#get_best_config_season_test(past_seasons, last_season)
#train = input[all_features]
#clf = RandomForestClassifier(max_depth=4, n_estimators=400, bootstrap=False)
#run_feature_importance_experiment(train, y, clf)

print("No Odds!")
#get_best_logistic_config()

get_best_config_season_test(input)
#get_best_config()