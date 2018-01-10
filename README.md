# ImagineCup2018
Imagine Cup 2018 project / 2018 이매진컵 프로젝트

[ImagineCup 개발배경](https://github.com/ninetyfivejae/ImagineCup2018/wiki/Imagine-Cup-2018-%EC%95%84%EC%9D%B4%EB%94%94%EC%96%B4-%EC%A0%95%EB%A6%AC)

---

## UI
- 전반적인 UI 틀 설계
	- [참고 오픈소스](https://github.com/Abel13/AnimatedMenu1)

- Main Page
![Main Page](http://cfile30.uf.tistory.com/image/99BAE5415A560AE2285DCE)

- Side menu board
![Side menu board](http://cfile28.uf.tistory.com/image/99FBF5335A560B1703F8BA)

---

## Pretrained google news data Word2Vec model 사용
- C#프로그램에서 파이썬 코드 실행 후 string으로 argument 전달
- ImagineCup2018 프로젝트 Environment.CurrentDirectory에 파일 위치
	- C:\Users\jae\Documents\ImagineCup2018\ImagineCupProject\ImagineCupProject\bin\Debug
- [google pretrained model DOWNLOAD](https://drive.google.com/file/d/0B7XkCwpI5KDYNlNUTTlSS21pQmM/edit)
	- 다운로드한 이후에 Environment.CurrentDirectory에 파일 위치에 옮겨놓으면 된다.
- WordClassification.py
```
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
```

---
