# E-Call Assistant
## E-Call Assistant / Imagine Cup 2018 project / 2018 이매진컵 프로젝트

![E-CallIcon](http://cfile29.uf.tistory.com/image/9994ED475A9550B018E726)

### [ImagineCup 개발배경](https://github.com/ninetyfivejae/ImagineCup2018/wiki/Imagine-Cup-2018-%EC%95%84%EC%9D%B4%EB%94%94%EC%96%B4-%EC%A0%95%EB%A6%AC)

---

## System Flow

![SystemFlow](http://cfile5.uf.tistory.com/image/99E364385AFFE4672C11B6)

### 1. 상황접수

긴급신고가 들어오면 프로그램을 통해서 최초 상황을 접수하게 됩니다.
최초 접수 시간부터 출동 지시 시간, 접수 전화를 끊은 시간까지 기록이 됩니다.
신고자가 당황하지 않고 침착하게 설명을 할 수 있도록 근무자가 필수적인 질문으로 진술을 유도합니다.

### 2. 신고자 진술 실시간 기록

긴급신고전화를 접수하는 순간부터 실시간으로 신고자의 진술내용이 기록이 됩니다.
이후에 Azure Cognitive Services 개체명 분석을 통해서 출동을 하기위해 필수적인 정보들을 분석해내고 해당하는 항목란에 채워줍니다.
출동을 지시할 수 있는 필수적인 항목들을 다 채우게 되면 이에따라 출동 지시를 내릴 수 있습니다.
그렇지 않은 경우에 알림을 통해서 근무자가 놓치고 있는 점을 알려줍니다.
근무자는 실시간으로 기록되는 내용에 추가적인 내용을 덧붙일 수 있습니다.

### 3. 신고 내용 분석

출동을 지시하는 과정에서, 핵심적인 신고 내용을 추출해서 딥러닝을 활용해 무슨 내용인지 분석을 합니다.
분석한 결과에 따라 신고내용이 세부적인 카테고리로 나누어지고 신고내용에 적절한 대응 매뉴얼이 화면에 제시됩니다.
분류된 신고내용에 따라 해당하는 대응 방식을 제시하여 주고 매뉴얼에 따라 세부적인 상황 내용을 질문합니다.
이렇게 얻어진 정보는 데이터베이스에 저장이 되어 출동기관으로 전송됩니다.

### 4. 신고자 상황대처 지시 및 출동대원 출동 지시

여러 상황에 맞게 적절한 대응 방식이 프로그램에서 제시가 됩니다.
또한, 부상자가 있는 경우 구조대 도착 전 대처법까지 알려주어 상황실 근무자는 당황하지 않고 전문적으로 신고자를 안심시키고 신고를 처리할 수 있습니다.

### 5. Azure Database

신고자의 진술 내용과 분석된 신고 내용은 Azure Database에 저장되고 관리됩니다.
또한, 실시간 상황실의 전반적인 신고 접수 현황을 그래프로 나타내주어 상황실 근무자가 인지를 할 수 있게 합니다.

---

## UI
### 전반적인 UI 틀 설계

- [참고 오픈소스](https://github.com/Abel13/AnimatedMenu1)

### Main Page

![Main Page](http://cfile21.uf.tistory.com/image/990FA53A5AFFE3E21D71EC)

### MainPage Click to Answer Page
- 전화가 오면 이 화면이 나오고 클릭하면 통화를 받게 된다.

![Main Page Click to Answer](http://cfile5.uf.tistory.com/image/99CC11375AFFE3E3104ED4)

### Classified Manual Page with additional questions

![Classified Manual Page with additional questions](http://cfile3.uf.tistory.com/image/99E36F3A5AFFE3E12295A5)

### Medical Response Manual Page

![Medical Response Manual Page](http://cfile27.uf.tistory.com/image/99B010395AFFE52A127FDF)

### Toast Notification

![ToastNotification](http://cfile30.uf.tistory.com/image/9995D5375AFFE3E315F499)

---

## Database

### Azure SQL Database

![Database flow](http://cfile23.uf.tistory.com/image/99929E345AFFE3DA10BC16)

신고 접수 및 신고 종료 시간과 세부적인 신고 접수내용을 Azure SQL 데이터베이스 서버에 전송시켜 관리
합니다.
근무자가 상황을 접수하면서 초기 상황 접수부터 세부적인 상황정리, 응급처치까지 크게 세 단계로 상 황을 정리해나가는데, 각 단계를 넘어갈 때마다 정리된 상황 내용이 데이터베이스에 저장이 됩니다.
저장된 상 황 내용은 Azure 서버를 통해 상황 공유가 필요한 담당기관으로 신속하게 전달됩니다.
또한, 데이터를 그래프 로 나타내어 상황실 근무자가 긴급 신고 상황에 대해 시각적으로 파악할 수 있게 구성했습니다.

### Data visualization with graph

![Graph Page](http://cfile21.uf.tistory.com/image/99C03F3A5AFFE3E112FCD8)

---

## STT, Text Analytic, POS Tagging
![STT, Text Analytic, POS Tagging](http://cfile8.uf.tistory.com/image/99434D345AFFE3DC17F42B)

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

## Improving STT accuracy algorithm
![Improving STT accuracy algorithm](http://cfile5.uf.tistory.com/image/99AC6C3A5AFFE3DE132CB6)

## Data Mining / Scraping Guardian news

- 가디언지 뉴스 데이터 ==> 정제된 뉴스 데이터

<img src="http://cfile2.uf.tistory.com/image/9926D6345AFFE3D719E854" width="50%"><img src="http://cfile21.uf.tistory.com/image/9933B7345AFFE3D82B86DE" width="50%">

- [참고 블로그 / Text classification using CNN written in tensorflow](http://manishankert.blogspot.kr/2017/04/text-classification-using-cnn-writte-in.html)
- 위 링크에서 가디언 뉴스 기사를 크롤링해와서 해당하는 카테고리의 기사로 학습을 시킴
- 사건 및 사고에 대해 신고자가 진술을 하고 상황을 설명하는 것이, 뉴스 기사에서 사건 및 사고에 대해서 설명을 하는 것과 유사성을 많이 찾을 수 있을 것이라 판단하여 뉴스 기사로 학습시키는 것을 선택했다.
- Goose와 BeautifulSoup4를 사용해서 기사 내용을 가져옴
- Google 검색에서 ```(category) site:www.guardian.com```이라고 검색하면 해당 사이트의 검색 결과만을 반환한다. 따라서, 구글에서 검색해서 나온 가디언지 링크들을 들어가서 기사 내용들을 가져온다.

## CNN Text Classification

![CNNTextClassification](http://cfile29.uf.tistory.com/image/998C673A5AFFE3DF03C898)

- [참고 문헌 / Implementing a cnn for text classification in tensorflow](http://www.wildml.com/2015/12/implementing-a-cnn-for-text-classification-in-tensorflow/) 
- 현재 75% 정확도 / 아직 학습해야할 카테고리가 더 있음 / 정확도 더 올릴 예정
- Training Command Example : ```python train.py ./data/train.csv ./parameters.json```
- Console Predict Command Example : ```python predict.py ./trained_model_1516543243/```
- JSON file Predict Command Example : ```python predict.py ./trained_model_1516543243/ ./data/sample.json```
- C# 프로젝트 bin-Debug-(Environment.CurrentDirectory)에 "data_helper.py", "predict.py", "text_cnn.py", "train.py", trained model 위치 시켜서 실행하기
- 예측 값
![CNNTextClassificationPrediction](http://cfile9.uf.tistory.com/image/99E6DA345AFFE3D80A1106)
- 정확도
![CNNTextClassificationAccuracy](http://cfile10.uf.tistory.com/image/998359345AFFE3D925CBC2)

## CNN Text Positive Negative Classification
- [참고 문헌 / Implementing a cnn for text classification in tensorflow](http://www.wildml.com/2015/12/implementing-a-cnn-for-text-classification-in-tensorflow/) 
- 현재 97% 정확도
- Training Command Example : ```python train_posneg.py```
- Console Predict Command Example : ```python eval_posneg.py --eval_train --checkpoint_dir="./runs/1516169064/checkpoints/"```
- C# 프로젝트 bin-Debug-(Environment.CurrentDirectory)에 "data_helpers_posneg.py", "eval_posneg.py", "text_cnn_posneg.py", "train_posneg.py", trained model 위치 시켜서 실행하기
- 윈도우에서 Data 파일 불러올 때, "UnicodeDecodeError: 'cp949' codec can't decode byte 0xe2 in position ---: illegal multibyte sequence" 오류 시, ```open('파일경로.txt', 'rt', encoding='UTF8')``` 이렇게 파일 Open하면 된다.
- 정확도
![PositiveNegativeAccuracy](http://cfile22.uf.tistory.com/image/990DCA345A7DA83E3C986C)

## 신고 대응 매뉴얼 참고자료 출처
- https://www.phoenix.gov/hrsite/Documents/fireguide.pdf
- http://www.vcp.state.va.us/pdfFiles/Emergency_Coordinator_Manual.pdf
- https://www.kerncounty.com/cao/policy/16.pdf