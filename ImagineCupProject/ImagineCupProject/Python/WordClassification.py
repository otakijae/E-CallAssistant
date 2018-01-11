import sys
import gensim
from threading import Semaphore

'''
#Load Google's pre-trained Word2Vec model.
model = gensim.models.KeyedVectors.load_word2vec_format('GoogleNews-vectors-negative300.bin', binary=True)
model.init_sims(replace=True)
model.save('GoogleNews-vectors-gensim-normed.bin')
'''

model_a = gensim.models.KeyedVectors.load('GoogleNews-vectors-gensim-normed.bin', mmap='r')
model_a.syn0norm = model_a.syn0  # prevent recalc of normed vectors

keyWords = sys.argv[1]

try:
    splittedKeyWords = keyWords.split(',')
    print(splittedKeyWords)
    print(model_a.most_similar(positive = splittedKeyWords))
    #print(model.most_similar_to_given(word, ['fire', 'watersupply', 'woman', 'teenager', 'gas', 'suicide', 'elder', 'child', 'disaster', 'rescue']))

except KeyError:
    print("Not in vocabulary")



'''
def WordClassification(object):

    model_a = gensim.models.KeyedVectors.load('GoogleNews-vectors-gensim-normed.bin', mmap='r')
    model_a.syn0norm = model_a.syn0  # prevent recalc of normed vectors

    try:
        #print(model_a.most_similar_to_given(word, ['fire', 'watersupply', 'woman', 'teenager', 'gas', 'suicide', 'elder', 'child', 'disaster', 'rescue']))
        li = []
        for key,val in model_a.most_similar(positive=DataPasser.input_li):
            li.append(key)
        output_li = li
        return ;
        #print(model_a.most_similar(positive=['teacher', 'highschool', 'student', 'gun', 'window'])) #using azure result
    except KeyError:
        print("Not in vocabulary")
        return ;
'''
#Semaphore(0).acquire()  # just hang until process killeds
#model = gensim.models.KeyedVectors.load_word2vec_format('GoogleNews-vectors-negative300.bin', binary=True, limit=5000)


#model_a = gensim.models.KeyedVectors.load('GoogleNews-vectors-gensim-normed.bin', mmap='r')
#model_a.syn0norm = model_a.syn0  # prevent recalc of normed vectors
#print(model_a.most_similar(positive=['king', 'woman']))
#li = []
#for key,val in model_a.most_similar(positive=['king', 'woman']):
#    li.append(key)
#print(li)
