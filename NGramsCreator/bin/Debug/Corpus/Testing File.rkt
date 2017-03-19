#lang racket

(read)

(define corpus (string->list (file->string "CrimeAndPunishment2.txt")))
(define corpus2 (string->list (file->string "WarAndPeace.txt")))

(define lexicon (make-hash))
(hash-set! lexicon "The" (make-hash))
(hash-ref lexicon "The")
(hash-set! (hash-ref lexicon "The") "apple" 1)
(hash-ref (hash-ref lexicon "The") "apple")
(hash-set! (hash-ref lexicon "The") "apple" 2)
(hash-ref (hash-ref lexicon "The") "apple")
(hash-set! (hash-ref lexicon "The") "bear" 2)
(hash-ref (hash-ref lexicon "The") "bear")
(hash-keys lexicon) ;; Use this to check if a word is in there... Have the function be all in one, as in if it gets to the
                     ;; end of the list, let it create a new hash
                      ;; if it finds it, set it to one higher




(integer->char 32)


(hash-keys (hash-ref lexicon "The")) ;; Create two separate lists here at the bottom
(hash-values (hash-ref lexicon "The"))



(define slist (list "a" "b" "c" "d"))


(struct bst-node (val left right) #:transparent #:mutable)

(define (bst-add tree value)
  (if (null? tree) (bst-node value null null)
      (let ([x (bst-node-val tree)])
        (cond
          [(= value x) tree]
          [(< value x) (if (null? (bst-node-left tree))
                       (set-bst-node-left! tree (bst-node value null null))
                       (bst-add (bst-node-left tree) value))]
          [(> value x) (if (null? (bst-node-right tree))
                       (set-bst-node-right! tree (bst-node value null null))
                       (bst-add (bst-node-right tree) value))])
        tree)))
        
(define (childless? node)
  (and (null? (bst-node-left node))
       (null? (bst-node-right node))))

(define (left-child-only? node)
  (and (not (null? (bst-node-left node)))
       (null? (bst-node-right node))))

(define (right-child-only? node)
  (and (null? (bst-node-left node))
       (not  (null? (bst-node-right node)))))
       
(define (bst-in? b n)
  (if (null? b) #f
      (let ([x (bst-node-val b)])
        (cond
          [(= n x) #t]
          [(< n x) (bst-in? (bst-node-left b) n)]
          [(> n x) (bst-in? (bst-node-right b) n)]))))

(define (bst? b)
  (define (between? b low high)
    (or (null? b)
        (and (<= low (bst-node-val b) high)
             (between? (bst-node-left b) low (bst-node-val b))
             (between? (bst-node-right b) (bst-node-val b) high))))
  (between? b -inf.0 +inf.0))
  
(define (bst-del tree value)
    (define (get-min-value tree)
      (cond
        [(null? (bst-node-left tree)) (bst-node-val tree)]
        [else (get-min-value (bst-node-left tree))]))
  
    (define (remove cur-node parent setter)
      (cond
        [(childless? cur-node) (setter parent null)]
        [(left-child-only? cur-node) (if (null? parent)
                                         (bst-node-left cur-node)
                                         (setter parent (bst-node-left cur-node)))]
        [(right-child-only? cur-node) (if (null? parent)
                                          (bst-node-right cur-node)
                                          (setter parent (bst-node-right cur-node)))]
        [else (let ([min (get-min-value (bst-node-right cur-node))])
                (set-bst-node-val! cur-node min)
                (start-removing (bst-node-right cur-node)
                                min
                                cur-node
                                set-bst-node-right!))]))
    
    (define (find-and-remove cur-node value parent setter)
      (let* ([x (bst-node-val cur-node)]
             [result
              (cond
                [(= value x) (remove cur-node parent setter)]
                [(< value x) (find-and-remove (bst-node-left cur-node) value cur-node set-bst-node-left!)]
                [(> value x) (find-and-remove (bst-node-right cur-node) value cur-node set-bst-node-right!)])])
        (if (void? result) tree result)))
  
    (define (start-removing tree value [parent null] [setter null])
      (if (bst-in? tree value)
          (find-and-remove tree value parent setter)
          tree))
  
    (start-removing tree value))
    
(define (list->bst l)
  (define (convert l) (cond
    [(= (length l) 1) (bst-node (first l) null null)]
    [(= (length l) 2)
     (if (list? (first l))
         (bst-node (second l) (list->bst (first l)) null)
         (bst-node (first l) null (list->bst (second l)) ))]
    [(= (length l) 3)
     (bst-node (second l)
               (list->bst (first l))
               (list->bst (third l)))]))
  (let ([b (convert l)])
    (if (bst? b) b
        (raise-argument-error 'list->bst "bst?" b))))