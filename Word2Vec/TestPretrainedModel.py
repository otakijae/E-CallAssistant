import gensim
from threading import Semaphore

'''
# Load Google's pre-trained Word2Vec model.
model = gensim.models.KeyedVectors.load_word2vec_format('GoogleNews-vectors-negative300.bin', binary=True)
model.init_sims(replace=True)
model.save('GoogleNews-vectors-gensim-normed.bin')
'''
model_a = gensim.models.KeyedVectors.load('GoogleNews-vectors-gensim-normed.bin', mmap='r')
model_a.syn0norm = model_a.syn0  # prevent recalc of normed vectors

#the model is loaded. It can be used to perform all of the tasks mentioned above.
word = input("Input word : ")
while(word != "exit") :
    try:
        print(model_a.most_similar_to_given(word, ['fire', 'watersupply', 'woman', 'teenager', 'gas', 'suicide', 'elder', 'child', 'disaster', 'rescue']))
        print(model_a.most_similar(positive=['grandfather', 'cancer', 'emergency']))
        print(model_a.most_similar(positive=['teacher', 'highschool', 'student', 'gun', 'window'])) #using azure result
        word = input("Input word : ")
    except KeyError:
        print("Not in vacabulary")
        word = input("Input word : ")

Semaphore(0).acquire()  # just hang until process killeds
#model = gensim.models.KeyedVectors.load_word2vec_format('GoogleNews-vectors-negative300.bin', binary=True, limit=5000)





'''
#LDA example
from nltk.tokenize import RegexpTokenizer
from stop_words import get_stop_words
from nltk.stem.porter import PorterStemmer
from gensim import corpora, models
tokenizer = RegexpTokenizer(r'\w+')
# create English stop words list
en_stop = get_stop_words('en')
# Create p_stemmer of class PorterStemmer
p_stemmer = PorterStemmer()
    
# create sample documents
doc_a = "Yes, I am a teacher at Columbine high school. There is a student here with a gun. He just shot out a window. I believe one um"
doc_b = "I don't know if it's. I don't know what's in my shoulder. If it was just some glass he threw or what"
doc_c = "I am. Yes, yes! And the school is in panic and I'm in the library. I've got students down under the tables. Kids! Heads under the tables."
doc_d = "Um, Kids are screaming. The teachers, um, are, you know, trying to take control of things. We need police here. "
doc_e = "Can you please hurry?"
doc_f = "I do not know who the student is."
doc_g = "... I was on hall duty, I saw a gun. I said, What's going on out there?"
doc_h = "And the kid that was following me said it was a film production, probably a joke"
doc_i = "And I said, well I don't think that's a good idea. And went walking outside to see what was going on."
doc_j = "He turned the gun straight at us and shot and, my god, the window went out. And the kid standing with me, I think he got hit"
doc_k = "Kids! Head down. I'am sorry?"
doc_l = "Okay. I'm in the library. He's upstairs. He's right outside here."
doc_m = "He's outside of this hall. There are alarms and things going off. There's smoke. My god smoke is coming into this room."
doc_n = "I've got the kids under the tables here. I don't know what's happening in the rest of the building."
doc_o = "I'm sure someone else is calling 911."
doc_p = "Okay. I'm on the floor."
doc_q = "In the library. And I've got every student in the library. On the floor! You got to stay on the floor!"
doc_r = "The gun is right outside of the library door, okay?"
doc_s = "I don't think I'm going to go out there, okay?"

# compile sample documents into a list
doc_set = [doc_a, doc_b, doc_c, doc_d, doc_e, doc_f, doc_g, doc_h, doc_i, doc_j, doc_k, doc_l, doc_m, doc_n, doc_o, doc_p, doc_q, doc_r, doc_s]

# list for tokenized documents in loop
texts = []

# loop through document list
for i in doc_set:
    # clean and tokenize document string
    raw = i.lower()
    tokens = tokenizer.tokenize(raw)
    print(tokens)

    # remove stop words from tokens
    stopped_tokens = [i for i in tokens if not i in en_stop]
    print(stopped_tokens)
    
    # stem tokens
    stemmed_tokens = [p_stemmer.stem(i) for i in stopped_tokens]
    print(stemmed_tokens)
    
    # add tokens to list
    texts.append(stemmed_tokens)
    print(texts)

# turn our tokenized documents into a id <-> term dictionary
dictionary = corpora.Dictionary(texts)
print(dictionary)
    
# convert tokenized documents into a document-term matrix
corpus = [dictionary.doc2bow(text) for text in texts]
print(corpus)

# generate LDA model
ldamodel = gensim.models.ldamodel.LdaModel(corpus, num_topics=3, id2word = dictionary, passes=50)
print(ldamodel.print_topics(num_topics=3, num_words=3))
'''





'''
#Gensim word2vec basic examples
#getting word vectors of a word
dog = model['dog']

#performing king queen magic
print(model.most_similar(positive=['woman', 'king'], negative=['man']))
print(model.most_similar(positive=['king', 'woman'], negative=['man']))
print(model.most_similar(positive=['basketball', 'golf'], negative=['michael', 'jordan']))
print(model.most_similar(positive=['matrix', 'dumb'], negative=['thoughtful']))

#picking odd one out
print(model.doesnt_match("breakfast cereal dinner lunch".split()))

#printing similarity index
print(model.similarity('woman', 'man'))
'''

'''
#'title' denotes the exact title of the article to be fetched
title = "Machine learning"
from wikipedia import page
wikipage = page(title)

from gensim.parsing import PorterStemmer
global_stemmer = PorterStemmer()
 
class StemmingHelper(object):
    """
    Class to aid the stemming process - from word to stemmed form,
    and vice versa.
    The 'original' form of a stemmed word will be returned as the
    form in which its been used the most number of times in the text.
    """
 
    #This reverse lookup will remember the original forms of the stemmed
    #words
    word_lookup = {}
 
    @classmethod
    def stem(cls, word):
        """
        Stems a word and updates the reverse lookup.
        """
 
        #Stem the word
        stemmed = global_stemmer.stem(word)
 
        #Update the word lookup
        if stemmed not in cls.word_lookup:
            cls.word_lookup[stemmed] = {}
        cls.word_lookup[stemmed][word] = (
            cls.word_lookup[stemmed].get(word, 0) + 1)
 
        return stemmed
 
    @classmethod
    def original_form(cls, word):
        """
        Returns original form of a word given the stemmed version,
        as stored in the word lookup.
        """
 
        if word in cls.word_lookup:
            return max(cls.word_lookup[word].keys(),
                       key=lambda x: cls.word_lookup[word][x])
        else:
            return word
'''
