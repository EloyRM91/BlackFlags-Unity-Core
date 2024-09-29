# Intercepción de barcos

La **intercepción** o cálculo de **trayectoria de intercepción** es una mecánica de movimiento introducida desde la alpha v0.0 que permite calcular el rumbo de persecución para alcanzar y atacar un objetivo. El jugador debe de entrar en trayectoria de interceptación si quiere atacar otro navío o convoy, haciendo click derecho sobre el objetivo. Asimismo, el jugador puede ser perseguido o atacado (en cuyo caso, el jugador será el objetivo de la intercepción).

Utilizamos el término **interceptación** ya que la unidad persecutora en ningún momento se dirige a la posición actual del objetivo si no a la posición en la que el objetivo se encontrará en un determinado instante de tiempo.

Dado que una unidad no puede atacar a otra unidad fuera de su alcance, primero debe de interceptar a la unidad objetivo. Para ello se realiza un cálculo en base a la velocidad y la posición de ambas unidades.

<p align="center">
  <img width="495" height="400" src="../../Assets/markdown-assets/gif/intercept-gif1.gif" alt="Un jugador interceptando un bergantín">
</p>

El cálculo en cuestión es simple: dado que la posición del perseguidor, la posición del objetivo, y la posición final de ambos barcos conforman un triángulo imaginario, con los datos de los que disponemos se puede calcular la posición final utilizando el [teorema del coseno](https://es.wikipedia.org/wiki/Teorema_del_coseno).

La unidad atacante pasa entonces a perseguir al objetivo, y si el objetivo puede ser alcanzado dada su velocidad, entonces tendrá lugar la interceptación. En ese caso, el punto final del rumbo determina la posición en el mapa donde ambas unidades se encuentran en la misma posición. Es decir, la posición en la cual el atacante intercepta al objetivo.

## Objetivos

La **interceptación** pertenece a la lógica de programación que corresponde a las mecánicas de movimiento de las unidades del juego.

Se quiere que el cálculo de rumbo de persecución sea exacto, evitando que el jugador tenga que realizar un esfuerzo por conducir su nave hacia el objetivo que quiera atacar. Igualmente, las patrullas NPC deben de ser capaces de perseguir al jugador sin errores.

Durante la navegación por el mapa, las unidades deben de redirigir su trayecto para sortear cabos o islotes (no siempre se puede navegar en línea recta). La intercepctación debe de producirse correctamente aún teniendo lugar estos cambios de rumbo. Por lo tanto, el juego debe de calcular nuevamente el punto de interceptación si la dirección del perseguidor cambia.

Igualmente, si el objetivo cambia de rumbo, el rumbo o trayectoria de persecución debe ser recalculado.

## Alcance de aplicación

- Detección de input: el jugador hace click sobre la unidad que quiere atacar.
- Respuesta de UI: Al hacer click sobre la unidad, la textura de canvas de esa unidad pega un parpadeo.
- Cálculo de interceptación: el script realiza la lógica matemática para obtener una posición.
- Movimiento de unidad: la nave del jugador se mueve hacia el punto designado.
- Detección de intercepción con éxito: la nave del jugador se detiene cuando ha alcanzado el objetivo (pasando a atacar)
- Ajustes de rumbo: el barco debe de poder reajustar su rumbo para perseguir correctamente al objetivo, tanto si aparecen obstáculos, como si es el objetivo el que encuentra un obstáculo.

Todo ello evitando posibles errores:

- Evitar siempre un path erróneo (intentar entrar en tierra o fuera del mapa)
- ¿Se puede perder la referencia al target? ¿Cómo evitar una excepción de referencia nula?

## Pseudocódigo

<p align="center">
  <img width="400" src="../../Assets/markdown-assets/svg/Diagrama%20-%20chase%20states.svg" alt="Diagrama de estados al mover o perseguir">
</p>

<p align="center">
  <img width="600" src="../../Assets/markdown-assets/svg/Diagrama%20-%20interception.svg" alt="Diagrama de flujo al interceptar a una unidad">
</p>

## Matemática requerida

Cuando se traza un rumbo de intercepción estamos dibujando un triángulo imaginario compuesto de tres puntos del mapa: la posición del perseguidor, la posición del perseguido, y la posición final de ambos. Es decir, si calculamos que la intercepción es posible (hablamos más adelante de casos en los que no sea posible interceptar a una unidad) entonces siempre existirá una posición en el espacio donde, transcurrido cierto tiempo, ambos barcos estarán en la misma posición del mapa.

Por lo tanto, en nuestro triángulo imaginario, o triángulo de intersección, tendremos un lado de longitud conocida, correspondiente a la distancia inicial entre los dos barcos, y dos lados de longitud no conocida previamente, que corresponden a la distancia recorrida de cada unidad.

Además, sabemos que podemos expresar la distancia como $${\displaystyle d = v · t}$$

También conocemos la dirección que toma el barco que será perseguido, así como su velocidad (concretamente su velocidad inicial, que supondremos que es constante durante el trayecto).

En definitiva, los datos que conocemos inicialmente son:

- La posición inicial del perseguidor,
- La posición inicial de la unidad o convoy perseguido
- Por extensión, de lo anterior ya conocemos la longitud de un lado de nuestro triángulo.
- Las velocidades de las dos unidades.
- La dirección a la que se dirige la unidad perseguida.

Por tanto, las incógnitas que queremos conocer son:

- La dirección que debe tomar la unidad perseguidora.
- El tiempo de intercepción, que determina la distancia recorrida durante la persecución, y por ende la longitud de los dos lados restantes del triángulo.

Si dibujamos solamente lo que conocemos a priori, tendríamos el esquema de la siguiente figura.

<p align="center">
  <img width="600" src="../../Assets/markdown-assets/svg/Vectores - interception 1.svg" alt="Triángulo de intercepción">
</p>

En el caso que he representado, un barco pirata (ya sea el del jugador, o un barco NPC) tiene que atacar a un mercante. Evidentemente la mecánica de movimiento al perseguir a una unidad no puede consistir en un LookAt. En un rumbo real de interceptación, no pondríamos el forward del objeto apuntando hacia donde está el objetivo, si no hacia donde estará en el futuro. Un comportamiento "chaser" del primer tipo sería el de un misil termoguiado. Un comportamiento del segundo tipo sería disparar una flecha hacia un objetivo en movimiento, alcanzándolo. Nosotros buscamos un comportamiento de los barcos NPC del segundo tipo; una intercepción "tipo flecha".

Según lo rápida que sea nuestra flecha, tardaremos menos en alcanzar al objetivo. Por ejemplo, en el siguiente esquema nuestro barco pirata traza diferentes triángulos de intercepción a diferentes velocidades. A una velocidad V1, el barco pirata tarda un tiempo t1 en alcanzar a su presa, que recorre una distancia Vm · t1. Si el barco pirata tiene una velocidad V2 menor que V1, le costará más alcanzar al objetivo, que recorrerá una distancia Vm · t2. Donde t2 evidentemente es mayot que t1 (hemos tardado más en alcanzar nuestra presa).

<p align="center">
  <img width="600" src="../../Assets/markdown-assets/svg/Vectores - interception 2.drawio.svg" alt="Triángulo de intercepción">
</p>

Tenemos claro lo que queremos conseguir, pero no sabemos hacia dónde disparar la flecha aún; o lo que es lo mismo: hacia dónde debe dirigirse el barco pirata, en nuestor caso. Está claro que si nuestro barco tiene una velocidad Vp, y el objetivo tiene una valocidad Vm, entonces existirá un tiempo T para el cual el barco pirata recorre una distancia Vp · T mientras el objetivo recorre una distancia Vm · T. Para ese tiempo T se puede dibujar un triángulo compuesto por la distancia inicial entre las dos unidades, la distancia recorrida por el pirata, y la distancia recorrida por el mercante, como se ve en la figura de abajo.

<p align="center">
  <img width="600" src="../../Assets/markdown-assets/svg/Vectores - interception 3.svg" alt="Triángulo de intercepción">
</p>

Esto nos deja con dos lados de magnitud desconocida, un lado del triángulo de longitud conocida, y un ángulo conocido: el ángulo alfa comprendido entre el segmento PM, y el segmento PI. Dicho de otro modo: conocemos las posiciones iniciales de las dos unidades y la dirección hacia la que se dirige el mercante, lo que determina el valor de alfa.

Valores conocidos:

- α
- PM = distancia inicial

<p align="center">
  <img width="400" src="../../Assets/markdown-assets/svg/Vectores - interception 4.svg" alt="Triángulo de intercepción">
</p>

Voy a hacer lo siguiente: como evidentemente este triángulo tiene infinitos triángulos semejantes a él, habrá uno en el que sea más fácil resolver el problema. Correcto: si divido por t la magnitud de todos los lados tengo un triángulo semenjante con los mismos ángulos, pero ahora el problema se simplifica: conozco el módulo de los lados PI y MI, mientras el segmento PM tiene un valor expresable como la distancia d / t, donde de es un valor conocido.

Valores conocidos:

- α
- PI = Vp
- MI = Vm

<p align="center">
  <img width="400" src="../../Assets/markdown-assets/svg/Vectores - interception 5.svg" alt="Triángulo de intercepción">
</p>

En este punto de la historia debería de darnos un chispazo de intuición, seguido de un trauma de Vietnam: Exacto, podemos usar la maldita ecuación del coseno, que no sabíamos para lo que iba a servir en la vida real, y resulta que es para que lo piratas puedan asaltar barcos :v

$${\displaystyle c^{2}=a^{2}+b^{2}-2ab\cos \gamma }$$

En donde el lado c corresponde al lado opuesto al ángulo gamma. Como tenemos un ángulo conocido, alfa, cuyo lado opuesto es el segmento PI = Vp, entonces nuestra ecuación quedaría así:

$${\displaystyle PI^{2}=PM^{2}+MI^{2}-2PM·MI\cos \alpha }$$

$${\displaystyle Vp^{2}=(d/t)^{2}+Vm^{2}-2(d/t)·Vm\cos \alpha }$$

Llegados a este punto, podemos simplificar la expresión como una ecuación de sengundo orden:

$${\displaystyle d/t=x }$$

$${\displaystyle x^{2}-2x·Vm\cos \alpha +Vm^{2}-Vp^{2} = 0}$$

$${\displaystyle x^{2}+bx+c=0}$$

$${\displaystyle x = \frac{-b +\sqrt{b^{2} - 4c}}{2}}$$

Aquí tendré en cuenta dos consideraciones:

- La formulación tradicional de la ecuación de segundo grado utiliza el operador ± por razones obvias: mi ecuación de segundo grado resuelve las raíces de un polinomio de segundo orden, así que tengo dos soluciones posibles. Yo voy a quedarme sólamente con la raíz mayor, por lo que utilizo el operador +.
- Además esta ecuación ya me indica en qué casos el pirata no puede alcanzar al mercante, dado que habrá casos en la que esta ecuación no tenga solución (o más concretamente, no devuelva raíces reales). Si no obtenemos una solución real, significa que no podremos trazar ningún triángulo que sea solución a nuetro problema con las condiciones que hemos introducido.

En otras palabras, el pirata puede alcanzar al mercante cuando se cumpla la siguiente condición:

$${\displaystyle b^{2} - 4c > 0 → b^{2} > 4c}$$

Si esto te parece espesito recuerda que en el instituto tenías que resolver esto en dos minutos :3

## Scripts

Por suerte sólo tenemos que escribir la ecuación en código y olvidarnos de esto para siempre: la clase convoy.cs se encargará de gestionar las interceptaciones.

```c#
        protected virtual Vector3 Intercept(Convoy target)
        {
            target.isOnTarget = true;
            var D = Vector3.Distance(target.transform.position, transform.position);
            var alfa = Quaternion.Angle(
              target.transform.rotation,
              Quaternion.LookRotation(transform.position - target.transform.position)
              );
            float C1 = 2 * target.convoySpeed * Mathf.Cos(alfa * Mathf.Deg2Rad);
            float C2 = Mathf.Pow(target.convoySpeed, 2) - Mathf.Pow(convoySpeed, 2);
            float C3 = Mathf.Pow(C1, 2) - 4 * C2;
            if (C3 < 0)
            {
               //No solution
                return transform.position + target.transform.forward * 15;
            }
            else
            {
                //Destination
                float t = 2 * D / (C1 + Mathf.Sqrt(C3));
                return target.transform.position + target.transform.forward * t * target.convoySpeed;
            }
        }
```

La función anterior devuelve la posición en el mapa a la que debe ir el persecutor para alcanzar a su objetivo.

Si no hay modo de alcanzar a la unidad, entonces la función traza un rumbo paralelo a ella. Si por el contrario la función calcula que la intercepción es posible, entonces averigua el valor t (el tiempo de intercepción) y, partiendo de la posición M del objetivo, calcula la posición final tomando la dirección a la que va el barco, la velocidad y el tiempo que el perseguidor tardará en alcanzarlo.

Un ejemplo de llamada a la función intercept sería el siguiente fragmento de código, que pertenece a una máquina de estados sencilla.

```c#

  //cosas...

  ...
        if (thisConvoyTarget != null)
        {
          if (Vector3.Distance(transform.position, thisConvoyTarget.transform.position) < 0.245f)
          {
            //alcanza el objetivo
            StopAllCoroutines();
            print("ataco al jugador o algo");
            }
            else if (i < waypoints.Length - 1 && i != 1)
            {
              if (Vector3.Distance(_destination, waypoints[i + 1]) > 0.5f)
              {
                var tempDest = Intercept(thisConvoyTarget);
                transform.rotation = Quaternion.LookRotation(waypoints[i + 1] - transform.position);
                _thisConvoySpriteController.SetSprite();
              }
            }
          }

  //cosas...
```

Con esto tenemos un código altamente modulado por niveles: la función intercept se encarga de devolver un valor.
Esta función es llamada bajo determinadas condiciones, como puede ser el estado de una máquina de estados del controlador de una unidad. 

```
//Pseudocódigo

Controlador de IA --> FSM (Máquina de estados)
"Ok máquina de estados, haz cosas mientras yo gestiono todo".

FSM (Máquina de estados) --> Estado "Chase" (Perseguir)
¡Voy a perseguir!

Estado "Chase" (Perseguir) --> Intercept (Método)
La decisión de atacar implica llamar al método.

```
