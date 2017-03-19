- Jon Skidanov 
- 04/27/2016
- Stochastic Language Generating Agent


Agknowledgements:
Wayne Iba - Provided the corpus of texts used in the program


Instructions:

To start generating random sentences:

1. Open up the "LanguageGeneration.rkt" file and run it.
2. When prompted to, type in one of the given files into the prompt to decide which ditionary 
   you would like to use.
3. After that, type into the prompt the number of times you would like it to generate a 5 N-Gram
   probabilistic language tree.
4. For full books it will take a while. After your paragraph has been created you can type "Y" or "N"
   to create another one just like it if you wish.


Notes:

-Versions of texts like "CrimeAndPunishment2.txt" will be smaller and easier to process

-For full books, prepare to wait a minute or two I was trying to implement a tree-sort function for productivity 
 but just needed to get something that works sent in.

-There may be some errors in my parsing and deciphering of the puntuation, but I think the way my program
 stores a 5 level deep hash set will be representative of my grasp on the concepts explored here. 
 Grabbing the information is a parsing issue, but the way I'm sorting, utilizing, and accessing the data follows 
 what was asked of us. I hope the infrastructure of the nested hash-set is what is seen, as that is one of the 
 most important parts. Even if the sentences look jumbled, it's searching through weighted trees under the hood.