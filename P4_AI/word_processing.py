from nltk.corpus import floresta
from nltk.corpus import stopwords
from nltk.tokenize import word_tokenize
import string

stopwords = stopwords.words('portuguese')

st = "eu gosto de me sentar na praia"



def simplify_tag(t):
    if "+" in t:
        return t[t.index("+")+1:]
    else:
        return t


for sent in floresta.tagged_sents():
    if 3 < len(sent) < 15:
        current_segment = []
        segments = []
        last_verb = ""
        for w, t in sent:
            if w not in string.printable:
                current_segment.append(w)
                if "v-fin" in simplify_tag(t):
                    segments.append(current_segment)
                    current_segment = []
        segments.append(current_segment)
        segments = reversed(segments)
        final = []
        for s in segments:
            final += s

        final.append(sent[len(sent)-1][0])

        print(" ".join(final).upper())
        print(" ".join(s for s, t in sent))
        print(" ".join(simplify_tag(t) for s, t in sent))
        print("----------------------------------------")






