import pandas as pd
import numpy as np
import matplotlib.pyplot as plt

input = pd.read_csv("data.csv")

features = ['HomeValue','AwayValue','HomePtsSeas','AwayPtsSeas','HomePtsLast','AwayPtsLast','HomePtsHome','AwayPtsAway','HomeGoalsSeas','AwayGoalsSeas','HomeGoalsLast','AwayGoalsLast','HomeGoalsHome','AwayGoalsAway','HomeGoalsConcSeas','AwayGoalsConcSeas','HomeGoalsConcLast','AwayGoalsConcLast','HomeGoalsConcHome','AwayGoalsConcAway','HomeTotalShots','AwayTotalShots','HomeShotsTarget','AwayShotsTarget','HomeTotalShotsAgst','AwayTotalShotsAgst','HomeShotsTargetAgst','AwayShotsTargetAgst', 'HomeTotalShotsVsTeam',	'AwayTotalShotsVsTeam',	'HomeShotsTargetVsTeam',	'AwayShotsTargetVsTeam','HomePossession','AwayPossession','HomeExpectedWin','AwayExpectedWin', 'HomeTrend', 'AwayTrend', 'HomeWeightedPoints',	'AwayWeightedPoints',	'ChanceCreationHome'	,'ChanceCreationAway',	'ShotsAccuracyHome',	'ShotsAccuracyAway',	'ScoringRateHome',	'ScoringRateAway',	'KeeperStrengthHome',	'KeeperStrengthAway', 'Round', 'HomeGoalsExpected',	'AwayGoalsExpected', 'HomeGoalsExpectedConc',	'AwayGoalsExpectedConc',	'HomeGoalsExpectedVsTeam',	'AwayGoalsExpectedVsTeam', 'HomeOdds',	'DrawOdds',	'AwayOdds']

print(input["HomeValue"].corr(input["HomePtsSeas"]))
print(input["HomeValue"].corr(input["HomeGoalsSeas"]))
print(input["HomeValue"].corr(input["HomeGoalsConcSeas"]))
print(input["HomeValue"].corr(input["HomeTotalShots"]))
print(input["HomeValue"].corr(input["HomeShotsTarget"]))
print(input["HomeValue"].corr(input["HomePossession"]))


print("Finished")