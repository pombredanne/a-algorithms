from __future__ import division
from numpy import *

def initialize(X, K):
    C = [X[0]]
    for k in range(1, K):
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


print initialize([1,2,3,4,5,6,7],3)