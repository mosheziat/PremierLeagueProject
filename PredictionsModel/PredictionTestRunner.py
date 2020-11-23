import pandas as pd
import numpy as np
from sklearn.ensemble import RandomForestClassifier
from sklearn.preprocessing import LabelEncoder
import csv

final_features = ['HomeValue','AwayValue','HomeShotsTargetVsTeam',	'AwayShotsTargetVsTeam', 'AwayShotsTarget',  'HomeShotsTarget', 'HomePossession', 'AwayPossession', 'HomeTotalShotsVsTeam', 'AwayTotalShotsVsTeam', 'AwayTotalShotsAgst', 'AwayWeightedPoints', 'HomeWeightedPoints', 'HomeGoalsExpectedVsTeam', 'AwayGoalsExpectedVsTeam', 'HomeGoalsSeas', 'AwayGoalsSeas', 'HomeTotalShots', 'AwayTotalShots', 'AwayGoalsExpected', 'HomeGoalsExpected', 'AwayGoalsExpectedConc', 'HomeGoalsExpectedConc', 'HomePtsSeas', 'AwayPtsSeas']
all_features = ['HomeValue','AwayValue', 'HomePtsSeas','AwayPtsSeas','HomePtsLast','AwayPtsLast','HomePtsHome','AwayPtsAway','HomeGoalsSeas','AwayGoalsSeas','HomeGoalsLast','AwayGoalsLast','HomeGoalsHome','AwayGoalsAway','HomeGoalsConcSeas','AwayGoalsConcSeas','HomeGoalsConcLast','AwayGoalsConcLast','HomeGoalsConcHome','AwayGoalsConcAway','HomeTotalShots','AwayTotalShots','HomeShotsTarget','AwayShotsTarget','HomeTotalShotsAgst','AwayTotalShotsAgst','HomeShotsTargetAgst','AwayShotsTargetAgst', 'HomeTotalShotsVsTeam',	'AwayTotalShotsVsTeam',	'HomeShotsTargetVsTeam',	'AwayShotsTargetVsTeam','HomePossession','AwayPossession', 'HomeTrend', 'AwayTrend', 'HomeWeightedPoints',	'AwayWeightedPoints',	'ChanceCreationHome'	,'ChanceCreationAway',	'ShotsAccuracyHome',	'ShotsAccuracyAway',	'ScoringRateHome',	'ScoringRateAway',	'KeeperStrengthHome',	'KeeperStrengthAway',  'HomeGoalsExpected',	'AwayGoalsExpected', 'HomeGoalsExpectedConc',	'AwayGoalsExpectedConc',	'HomeGoalsExpectedVsTeam',	'AwayGoalsExpectedVsTeam']
goals_all_features = ['AwayTeamOdds', 'DrawOdds', 'HomeTeamOdds', 'HomeValue','AwayValue', 'HomePtsSeas','AwayPtsSeas','HomePtsLast','AwayPtsLast','HomePtsHome','AwayPtsAway','HomeGoalsSeas','AwayGoalsSeas','HomeGoalsLast','AwayGoalsLast','HomeGoalsHome','AwayGoalsAway','HomeGoalsConcSeas','AwayGoalsConcSeas','HomeGoalsConcLast','AwayGoalsConcLast','HomeGoalsConcHome','AwayGoalsConcAway','HomeTotalShots','AwayTotalShots','HomeShotsTarget','AwayShotsTarget','HomeTotalShotsAgst','AwayTotalShotsAgst','HomeShotsTargetAgst','AwayShotsTargetAgst', 'HomeTotalShotsVsTeam',	'AwayTotalShotsVsTeam',	'HomeShotsTargetVsTeam',	'AwayShotsTargetVsTeam','HomePossession','AwayPossession', 'HomeTrend', 'AwayTrend', 'HomeWeightedPoints',	'AwayWeightedPoints',	'ChanceCreationHome'	,'ChanceCreationAway',	'ShotsAccuracyHome',	'ShotsAccuracyAway',	'ScoringRateHome',	'ScoringRateAway',	'KeeperStrengthHome',	'KeeperStrengthAway',  'HomeGoalsExpected',	'AwayGoalsExpected', 'HomeGoalsExpectedConc',	'AwayGoalsExpectedConc',	'HomeGoalsExpectedVsTeam',	'AwayGoalsExpectedVsTeam', 'IsHomeFavorite']
feats_1 = ['HomeValue', 'AwayValue', 'HomeGoalsHome', 'AwayGoalsAway', 'HomeGoalsConcHome', 'AwayGoalsConcAway', 'HomeGoalsExpected', 'AwayGoalsExpected', 'HomeGoalsExpectedConc', 'AwayGoalsExpectedConc', 'HomeTotalShotsVsTeam', 'AwayTotalShotsVsTeam', 'HomeShotsTargetVsTeam', 'AwayShotsTargetVsTeam', 'HomePtsSeas', 'AwayPtsSeas', 'HomeGoalsExpectedVsTeam', 'AwayGoalsExpectedVsTeam']

def run_test(train_file, test_file, features, goals_features, clf, goals_clf, file_path):
    print("Started Running Test")

    x_train = train_file[features]
    lb = LabelEncoder()
    target = train_file["Winner"]
    y_train = lb.fit_transform(target)

    clf.fit(x_train, y_train)
    print("Trained Prediction Model")

    x_test = test_file[features]
    y_proba = clf.predict_proba(x_test)
    print("Got Prediction Model Scores!")

    x_train2 = train_file[goals_features]
    target2 = train_file['Over2.5']
    y_train2 = lb.fit_transform(target2)
    goals_clf.fit(x_train2, y_train2)
    print("Trained Goals Model")

    x_test2 = test_file[goals_features]
    y_proba2 = goals_clf.predict_proba(x_test2)
    print("Got Goals Model Scores!")

    write_test_results(test_file, y_proba, y_proba2, file_path)



def write_test_results(test_file, y_proba, y_proba2, file_path):
    matches = test_file['Match']
    with open(file_path, 'w', newline='') as csvfile:
        spamwriter =csv.writer(csvfile, delimiter=',', quoting=csv.QUOTE_MINIMAL)
        headers = ["Match", "Winner Odds", "Away Odds", "Draw Odds", "Home Odds", "Predicted Winner", "Winner Ratio", "Bookie Ratio", "Diff", "Winner", "Model Correct", "Goals Odds", "IsOver", "Over Ratio"]
        spamwriter.writerow(headers)
        for i in range(len(y_proba)):
            to_write = []
            max1 = max(y_proba[i])
            print(matches[i])
            print(y_proba[i])
            to_write.append(matches[i])
            to_write.append(max1)
            print(max1)
            max_index = y_proba[i].tolist().index(max1)
            predicted = "D"
            if max_index == 0:
                predicted = "A"
            if max_index == 2:
                predicted = "H"
            print(predicted)
            to_write.extend(y_proba[i])
            to_write.append(predicted)
            to_write.append(100 / (max1 * 100))
            to_write.extend(["", "", "", ""])
            max2 = max(y_proba2[i])
            to_write.append(max2)
            max_index2 = y_proba2[i].tolist().index(max(y_proba2[i]))
            to_write.append(max_index2)
            to_write.append(100 / (max2 * 100))

            spamwriter.writerow(to_write)


train_file = pd.read_csv("train.csv")
#train_file = train_file.loc[train_file['Round'] > 0.04]

print("Train File:", train_file.shape)

test_file = pd.read_csv("test.csv")
print("Test File:", test_file.shape)

features = all_features
print(features)
threshold=  0.65
goals_features = all_features
print(goals_features)



clf = RandomForestClassifier(max_depth=50, n_estimators=2500, bootstrap=False)
goals_clf = RandomForestClassifier(max_depth=2, n_estimators=50, bootstrap=False)

run_test(train_file, test_file, features, goals_features=goals_features, clf=clf, goals_clf=goals_clf, file_path= "Round9B.csv")
