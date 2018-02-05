import codecs

UTF8Writer = codecs.getwriter('utf8')

import urllib2
import urlparse
from goose import Goose
from bs4 import BeautifulSoup, SoupStrainer

section_dict = {}
g=Goose()

OUTPUTFILE="emergency.txt"
OUTPUT_URLS="emergency.txt"

def get_urls(main_url):

    hdr = {'User-Agent': 'Mozilla/5.0', 'referer' : main_url}
    resp = urllib2.Request(main_url, headers=hdr)
    html = urllib2.urlopen(resp)
    html_doc = html.read()
    html.close()
    soup = BeautifulSoup(html_doc, 'html.parser')
    sections = soup.find_all('section')
    get_main_text(main_url)

def get_main_text(main_url):
    article = g.extract(url=main_url)
    with open(OUTPUTFILE, 'a') as ut:
        ut = UTF8Writer(ut)
        ut.write('\n###########################################################' + "\n")
        #ut.write("\n")
        ut.write(article.cleaned_text)

def main():
    for x in range(50):
        page_number = (x*10)
        #url = 'https://www.google.co.uk/search?q=disaster+site:www.theguardian.com&lr=&as_qdr=all&ei=sGpbWpbPH4Sb8QWb4KigDw&start=' + str(page_number) + '&sa=N&biw=760&bih=703'
        #url = 'https://www.google.co.uk/search?q=earthquake+site:www.theguardian.com&lr=&as_qdr=all&ei=_wBhWsC6BMWb0gTdh5WwAQ&start=' + str(page_number) + '&sa=N&biw=1280&bih=703&dpr=2'
        #url = 'https://www.google.co.uk/search?q=flood+site:www.theguardian.com&lr=&as_qdr=all&ei=eQlhWu2LCYbs0gSEhayYAQ&start=' + str(page_number) + '&sa=N&biw=1280&bih=703'
        #url = 'https://www.google.co.uk/search?q=severe+weather+site:www.theguardian.com&lr=&as_qdr=all&ei=5QlhWpH0GcWl0ATT_LSYDQ&start=' + str(page_number) + '&sa=N&biw=1280&bih=703'
        #url = 'https://www.google.co.uk/search?q=terror+site:www.theguardian.com&lr=&as_qdr=all&ei=YAphWuexHMSw0gSTpJ2IBg&start=' + str(page_number) + '&sa=N&biw=1280&bih=703'
        #url = 'https://www.google.co.uk/search?q=gunshot+site:www.theguardian.com&lr=&as_qdr=all&ei=pQphWtTJIoam0ASn04KIBQ&start=' + str(page_number) + '&sa=N&biw=1280&bih=703'
        #url = 'https://www.google.co.uk/search?q=bomb+site:www.theguardian.com&lr=&as_qdr=all&ei=_gphWtz-PITP0gS3x5TADg&start=' + str(page_number) + '&sa=N&biw=1280&bih=703'
        #url = 'https://www.google.co.uk/search?q=fire+site:www.theguardian.com&lr=&as_qdr=all&ei=MQthWr6gNovL0ASM27zACQ&start=' + str(page_number) + '&sa=N&biw=1280&bih=703'
        #url = 'https://www.google.co.uk/search?q=violence+site:www.theguardian.com&lr=&as_qdr=all&ei=lQthWp2jDIvL0ASM27zACQ&start=' + str(page_number) + '&sa=N&biw=1280&bih=703'
        #url = 'https://www.google.co.uk/search?q=women+violence+site:www.theguardian.com&lr=&as_qdr=all&ei=xwthWqjEAYyw0AT04pfIDQ&start=' + str(page_number) + '&sa=N&biw=1280&bih=703'
        #url = 'https://www.google.co.uk/search?q=teenage+violence+site:www.theguardian.com&lr=&as_qdr=all&ei=FQxhWvyrOsGa0gSE6L6AAw&start=' + str(page_number) + '&sa=N&biw=1280&bih=703'
        #url = 'https://www.google.co.uk/search?q=rescue+site:www.theguardian.com&lr=&as_qdr=all&ei=JO1hWqGLG8Kx0gTc4p-ICw&start=' + str(page_number) + '&sa=N&biw=1280&bih=703'
        #url = 'https://www.google.co.uk/search?q=suicide+site:www.theguardian.com&lr=&as_qdr=all&ei=eBFhWp7WEsi50ASy2YuoAQ&start=' + str(page_number) + '&sa=N&biw=1280&bih=703'
        #url = 'https://www.google.co.uk/search?q=cruel+treatment+site:www.theguardian.com&lr=&as_qdr=all&ei=lAxhWrjyNoOs0ATMxbH4Ag&start=' + str(page_number) + '&sa=N&biw=1280&bih=703'
        #url = 'https://www.google.co.uk/search?q=motor+vehicle+accidents+site:www.theguardian.com&lr=&as_qdr=all&ei=qBFhWtqqI8Gp0ATg7I_IDA&start=' + str(page_number) + '&sa=N&biw=1280&bih=703'
        url = 'https://www.google.co.kr/search?q=emergency+site:www.theguardian.com&ei=-9R2WprJMYXs0gSRvI_QCw&start=' + str(page_number) + '&sa=N&biw=1280&bih=703'

        hdr = {'User-Agent': 'Mozilla/5.0', 'referer' : url}
        req = urllib2.Request(url, headers=hdr)
        html = urllib2.urlopen(req)
        source = html.read()
        html.close()

        soup = BeautifulSoup(source, "html.parser")

        for a in soup.select('.r a'):
            get_urls(urlparse.parse_qs(urlparse.urlparse(a['href']).query)['q'][0])
        
if __name__ == '__main__':
    main()
