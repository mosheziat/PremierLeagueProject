import tensorflow as tf
import pandas as pd
import numpy as np
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import LabelEncoder
from sklearn.metrics import accuracy_score

input = pd.read_csv("data.csv")
#features = ['HomePtsSeas','AwayPtsSeas','HomePtsLast','AwayPtsLast','HomePtsHome','AwayPtsAway','HomeGoalsSeas','AwayGoalsSeas','HomeGoalsLast','AwayGoalsLast','HomeGoalsHome','AwayGoalsAway','HomeGoalsConcSeas','AwayGoalsConcSeas','HomeGoalsConcLast','AwayGoalsConcLast','HomeGoalsConcHome','AwayGoalsConcAway','HomeTotalShots','AwayTotalShots','HomeShotsTarget','AwayShotsTarget','HomeTotalShotsAgst','AwayTotalShotsAgst','HomeShotsTargetAgst','AwayShotsTargetAgst','HomePossession','AwayPossession','HomeExpectedWin','AwayExpectedWin']
features = ['HomeValue','AwayValue','HomePtsSeas','AwayPtsSeas','HomePtsLast','AwayPtsLast','HomePtsHome','AwayPtsAway','HomeGoalsSeas','AwayGoalsSeas','HomeGoalsLast','AwayGoalsLast','HomeGoalsHome','AwayGoalsAway','HomeGoalsConcSeas','AwayGoalsConcSeas','HomeGoalsConcLast','AwayGoalsConcLast','HomeGoalsConcHome','AwayGoalsConcAway','HomeTotalShots','AwayTotalShots','HomeShotsTarget','AwayShotsTarget','HomeTotalShotsAgst','AwayTotalShotsAgst','HomeShotsTargetAgst','AwayShotsTargetAgst','HomePossession','AwayPossession','HomeExpectedWin','AwayExpectedWin']

lb = LabelEncoder()
target = input["Winner"]
y = lb.fit_transform(target)

#y = input["isOver"]
#x = input.iloc[:,0:32]
x = input[features]