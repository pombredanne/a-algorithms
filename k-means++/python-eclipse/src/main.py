#An implementation of k-means++ for one dimensional vector

from __future__ import division
from numpy import *
#from sys import maxint

K=3
#centroids_completed = arange(K) > maxint

# This function is the difference campared to the original k-means
# returns which points from X are choose as centroids
def initialize(X, K):
    C = [X[0]]
    for _ in range(1, K):
        D2 = array([min([inner(c-x,c-x) for c in C]) for x in X])
        #print D2
        probs = D2/D2.sum()
        cumprobs = probs.cumsum()
        #print cumprobs
        r = random.rand()
        #print r
        i=-1
        for j,p in enumerate(cumprobs):
            if r < p:
                i = j
                break
        #print i
        C.append(X[i])
    return C

def calculate_cluster(c,X):
    #D2 = array([min([inner(c-x,c-x) for c in C]) for x in X])
    return

def kmeanspp():
    initial_centroids = initialize([1,2,3,4,5,6,7],K)
    print initial_centroids

kmeanspp()