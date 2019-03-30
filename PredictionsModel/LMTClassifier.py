from weka.classifiers import Classifier
import csv
from weka.classifiers import Evaluation
from weka.core.classes import Random

data = pd.read_csv("data2.csv")
data = shuffle(input)

cls = Classifier(classname="weka.classifiers.trees.LMT", options=["-C", "0.3"])
cls.build_classifier(data)


evl = Evaluation(data)
evl.crossvalidate_model(fc, data, 10, Random(1))

print(evl.percent_correct)
print(evl.summary())
print(evl.class_details())

