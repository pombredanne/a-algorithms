from __future__ import division

def readfile(filename):
  lines=[line for line in file(filename)]
  rownames=[]
  data=[]
  for line in lines:
    p=line.strip().split(' ') #single space as separator
    #print p
    # First column in each row is the rowname
    rownames.append(p[0])
    # The data for this row is the remainder of the row
    data.append([float(x) for x in p[1:]])
    #print [float(x) for x in p[1:]]
  return rownames,data

from math import sqrt

def pearson(v1,v2):
  # Simple sums
  sum1=sum(v1)
  sum2=sum(v2)

  # Sums of the squares
  sum1Sq=sum([pow(v,2) for v in v1])
  sum2Sq=sum([pow(v,2) for v in v2])

  # Sum of the products
  pSum=sum([v1[i]*v2[i] for i in range(len(v1))])

  # Calculate r (Pearson score)
  num=pSum-(sum1*sum2/len(v1))
  den=sqrt((sum1Sq-pow(sum1,2)/len(v1))*(sum2Sq-pow(sum2,2)/len(v1)))
  if den==0: return 0

  return 1.0-num/den

import numpy
from numpy.random import *

def initialize(X, K):
    C = [X[0]]
    for _ in range(1, K):
        #D2 = numpy.array([min([numpy.inner(c-x,c-x) for c in C]) for x in X])
        D2 = numpy.array([min([numpy.inner(numpy.array(c)-numpy.array(x),numpy.array(c)-numpy.array(x)) for c in C]) for x in X])
        probs = D2/D2.sum()
        cumprobs = probs.cumsum()
        #print "cumprobs=",cumprobs
        r = rand()
        #print "r=",r
        i=-1
        for j,p in enumerate(cumprobs):
            if r < p:
                i = j
                break
        C.append(X[i])
    return C

def kcluster(rows,distance=pearson,k=4):
  # Determine the minimum and maximum values for each point
  #ranges=[(min([row[i] for row in rows]),max([row[i] for row in rows]))
  #for i in range(len(rows[0]))]
  # Create k randomly placed centroids
  #clusters=[[random.random()*(ranges[i][1]-ranges[i][0])+ranges[i][0]
  #for i in range(len(rows[0]))] for j in range(k)]
  clusters = initialize(rows,k)
  print "initial centroids=",clusters

  lastmatches=None
  for t in range(100):
    print 'Iteration %d' % t
    bestmatches=[[] for i in range(k)]

    # Find which centroid is the closest for each row
    for j in range(len(rows)):
      row=rows[j]
      bestmatch=0
      for i in range(k):
        d=distance(clusters[i],row)
        if d<distance(clusters[bestmatch],row): bestmatch=i
      bestmatches[bestmatch].append(j)

    # If the results are the same as last time, this is complete
    if bestmatches==lastmatches: break
    lastmatches=bestmatches

    # Move the centroids to the average of their members
    for i in range(k):
      avgs=[0.0]*len(rows[0])
      if len(bestmatches[i])>0:
        for rowid in bestmatches[i]:
          for m in range(len(rows[rowid])):
            avgs[m]+=rows[rowid][m]
        for j in range(len(avgs)):
          avgs[j]/=len(bestmatches[i])
        clusters[i]=avgs

  return bestmatches

rows,data=readfile('d:\\data.txt')

kclust = kcluster(data,k=4)

print "Result:"
for c in kclust:
    out = ""
    for r in c:
        out+=rows[r] +' '
    print "["+out[:-1]+"]"

print 'done'

