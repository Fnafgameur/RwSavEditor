Pour reload la save afin que les effets du save editor s'applique :

- Relancer complètement son jeu
- Switch de save dans les options (exemple : save editor a modifié sur save 1 alors, switch sur save 2 et revenir sur save 1)


Les saves sont différenciés par la couleur du perso (à prouver pour DLC chars):

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
IN GAME CYCLE : 20   19   18   17   16   15   14   13   12   11   10    9      8      7      6      5      4      3      2      1      0
IN FILE CYCLE : -1   0     1    2    3    4    5    6    7    8    9    10     11     12     13     14     15     16     17     18     19


Les spups sont stockés dans la catégorie de la campagne et comportent leurs stats :

- Ils sont stockés dans ";FRIENDS&lt;svB&gt;"

- "cA&gt;ID.-1.8650&lt;" : leurs ID
- "cA&gt;HI_S05.0&lt;" : leurs positions
- ";Food&lt;cC&gt;1&lt;" : leurs nourriture ( /!\ Si <= 1 alors il sera en état de malnutrition, mettre 5 pour max food /!\ )
- ";Malnourished&lt;cC&gt;False&lt;" : défini si le spup meurt de malnutrition lors du chargement de la sauvegarde
- ";Pup&lt;cC&gt;True&lt;" : défini si c'est un enfant
- ";Glow&lt;cC&gt;False&lt;" : si il peut briller (effet après avoir mangé un neuronne)
- ";Mark&lt;cC&gt;False&lt;" : si il a la marque de communication (?)
- ";Karma&lt;cC&gt;0&lt;" : son karma (?)
- ";FullGrown&lt;cC&gt;False&lt;" : si c'est un adulte
- ";Stomach&lt;cC&gt;NULL&lt;" : l'item qu'il y aura dans son estomac



