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

## Data Mining / Scraping Guardian news

- [참고 블로그 / Text classification using CNN written in tensorflow](http://manishankert.blogspot.kr/2017/04/text-classification-using-cnn-writte-in.html)
- 위 링크에서 가디언 뉴스 기사를 크롤링해와서 해당하는 카테고리의 기사로 학습을 시킴
- 사건 및 사고에 대해 신고자가 진술을 하고 상황을 설명하는 것이, 뉴스 기사에서 사건 및 사고에 대해서 설명을 하는 것과 유사성을 많이 찾을 수 있을 것이라 판단하여 뉴스 기사로 학습시키는 것을 선택했다.
- Goose와 BeautifulSoup4를 사용해서 기사 내용을 가져옴
- Google 검색에서 ```(category) site:www.guardian.com```이라고 검색하면 해당 사이트의 검색 결과만을 반환한다. 따라서, 구글에서 검색해서 나온 가디언지 링크들을 들어가서 기사 내용들을 가져온다.

## CNN Text Classification
- [참고 문헌 / Implementing a cnn for text classification in tensorflow](http://www.wildml.com/2015/12/implementing-a-cnn-for-text-classification-in-tensorflow/) 
- 현재 75% 정확도 / 아직 학습해야할 카테고리가 더 있음 / 더 정확도 올릴 예정
- 기본 예제로 실행했을 시 예측 값
```
[
    {
        "Category": "Violence",
        "Descript": "Gunshot gangsters",
        "new_prediction": "Terror"
    },
    {
        "Category": "Violence",
        "Descript": "Knife killing people",
        "new_prediction": "Violence"
    },
    {
        "Category": "Terror",
        "Descript": "Yes I am a teacher at Columbine high school. There is a student here with a gun. He just shot out a window. I believe one um. I don't know if it's. I don't know what's in my shoulder. If it was just some glass he threw or what I am. Yes yes! And the school is in panic and I'm in the library. I've got students down under the tables. Kids! Heads under the tables.Um Kids are screaming. The teachers um are you know trying to take control of things. We need police here.  Can you please hurry?I do not know who the student is.  %u2026 I was on hall duty I saw a gun. I said What's going on out there?  And the kid that was following me said it was a film production probably a joke  And I said well I don't think that's a good idea. And went walking outside to see what was going on.  He turned the gun straight at us and shot and my god the window went out. And the kid standing with me I think he got hit  Kids! Head down. I'am sorry?  Okay. I'm in the library. He's upstairs. He's right outside here.  He's outside of this hall. There are alarms and things going off. There's smoke. My god smoke is coming into this room. I've got the kids under the tables here. I don't know what's happening in the rest of the building.  I%u2019m sure someone else is calling 911.  Okay. I'm on the floor.  In the library. And I've got every student in the library. On the floor! You got to stay on the floor!  The gun is right outside of the library door okay?  I don't think I'm going to go out there okay?  %u2026 Um I'm not going to go to the door. He just shot toward the door. Okay?",
        "new_prediction": "Violence"
    },
    {
        "Category": "Terror",
        "Descript": "school there is a student here with a gun he just shot out a window student is i was on hall kids under the tables student kids window library hall tables school teacher heads good joke production idea film duty shot hurry",
        "new_prediction": "Violence"
    },
    {
        "Category": "Terror",
        "Descript": "I think bomb is installed in the building. I'm scared. Someone called to our office and threatening us. Please come quickly. Help!",
        "new_prediction": "Fire"
    },
    {
        "Category": "Fire",
        "Descript": "I am the passenger and I see the 63 building is on fire. I think 119 needs to check this out quickly.",
        "new_prediction": "Fire"
    },
    {
        "Category": "Disaster",
        "Descript": "Tornado a typhoon. Earthquake. Mad.",
        "new_prediction": "Disaster"
    },
    {
        "Category": "Disaster",
        "Descript": "Footage captured by a drone shows the scale of flooding in Houston on Friday. The amateur video shows the aftermath of Hurricane Harvey as residents begin to return to undertake the massive cleanup effort. Donald Trump was due to meet survivors in Houston on Saturday",
        "new_prediction": "Disaster"
    },
    {
        "Category": "Disaster",
        "Descript": "A powerful \u201cweather bomb\u201d has hit New Zealand, cutting off rural towns, flooding major roads and dumping snow on to bare alpine ski fields at what should be the height of the southern hemisphere summer.",
        "new_prediction": "Disaster"
    },
    {
        "Category": "Disaster",
        "Descript": "My apartment is shaking so hard, what is it? Nothing about earthquake on the media. I'm scared",
        "new_prediction": "Disaster"
    },
    {
        "Category": "Disaster",
        "Descript": "I am burning out. I am studying hard.",
        "new_prediction": "Fire"
    },
    {
        "Category": "Motor vehicle accidents",
        "Descript": "A plane crashed in the Kimpo Airport.",
        "new_prediction": "Terror"
    },
    {
        "Category": "Motor vehicle accidents",
        "Descript": "car crashed in the main street. please come quickly",
        "new_prediction": "Motor vehicle accidents"
    }
]
```

## 신고 대응 매뉴얼 참고자료 출처
- https://www.phoenix.gov/hrsite/Documents/fireguide.pdf
- http://www.vcp.state.va.us/pdfFiles/Emergency_Coordinator_Manual.pdf
- https://www.kerncounty.com/cao/policy/16.pdf