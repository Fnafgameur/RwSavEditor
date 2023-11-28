Les différentes save sont différenciés par la couleur du perso (à prouver pour DLC chars):

Monk = Yellow
Survivor = White
Hunter = Red

ex:
Monk : "[...]SAV STATE NUMBER&lt;svB&gt;Yellow&lt;[...]"
Survivor : "[...]SAV STATE NUMBER&lt;svB&gt;White&lt;[...]"
Hunter : "[...]SAV STATE NUMBER&lt;svB&gt;Red&lt;[...]"


Les noms des attributs des saves (nb cycle, karma, food...) commencent par un "svA&gt;" et se ferment par un "svB&gt;" OU "dpA&gt" et "dpB&gt":

ex: 
FOOD : "[...]svA&gt;TOTFOOD&lt;svB&gt;[...]"
KARMA RENFORCE ? : "[...]dpA&gt;REINFORCEDKARMA&lt;dpB&gt;[...]"


La valeur de l'attributs se trouve juste après le mot clé fermant ("svB&gt;" OU "dpB&gt;"):

ex:
FOOD : "[...]svA&gt;TOTFOOD&lt;svB&gt;5&lt;[...]" 5 étant la valeur entière
donc FOOD : "[...]svA&gt;TOTFOOD&lt;svB&gt;{valeurInt}&lt;[...]"
KARMA RENFORCE ? : "[...]dpA&gt;REINFORCEDKARMA&lt;dpB&gt;1&lt;[...]" 1 étant la valeur booléenne (0: faux, 1: vrai)
donc KARMA RENFORCE ? : "[...]dpA&gt;REINFORCEDKARMA&lt;dpB&gt;{valeurBool}&lt;[...]"


Les cycles du hunter fonctionnent différemment des autres (+ on monte en cycle, + on descend en valeur jusqu'aux négatif):

ex:
100 cyles feront -100-19=-119 cycles (car hunter commence avec 19 cycles)