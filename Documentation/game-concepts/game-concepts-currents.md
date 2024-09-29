# Corrientes

¡Deja que te lleve la corriente con la introducción de las nuevas mecánicas en la versión [0.031](../versions/v0-031.md)! Uno de los cambios más interesantes son el **modificador de corrientes**, que como el nombre indica, va a afectar a la navegación, y el **mapa de corrientes**, que indica al jugador cuál es la corriente predominante en cada zona del mapa.


<p align="center">
  <img width="800"" src="../../Assets/markdown-assets/currents-example%201.jpg" alt="Corrientes predominantes en Isla de Cuba, Yucatán y la Florida">
</p>

<p align="center">
  <img width="800" src="../../Assets/markdown-assets/currents-example%202.png" alt="Mapa de corrientes">
</p>

<p align="center">
  <img width="800" src="../../Assets/markdown-assets/currents-example%203.jpg" alt="Corrientes y jugador">
</p>

Con la introducción de las corrientes en el juego se pretende dar mayor dimensión a las mecánicas de movimiento y microgestión, y además conferir una sensación de recompensa al jugador por medio del aprendizaje: El jugador que trace sus rutas de movimiento basándose en las corrientes tendrá una ventaja al atacar, ocultarse, comerciar o huir.

Además, las nuevas mecánicas se introducirán de manera progresiva en la curva de aprendizaje del jugador, de manera que las corrientes afectarán poco al desempeño del jugador al desplazarse por el mapa durante el early game, y se convertirá en un factor más importante -e incluso una ventaja si el jugador sabe aprovechar las corrientes- a partir del middle game. Esto se debe a que existirá una relación entre el tamaño de las embarcaciones y la influencia de la corriente sobre el barco: los barcos de muy poco calado recibirán una influencia casi despreciable de la corriente, mientras que los barcos más grandes sufrirán mayor efecto de la corriente. Esto hará que a comienzo de la partida el jugador, al tener una embarcación ligera y buscar presas pobremente armadas y de poco calado, pueda ignorar la repercusión de las corrientes en el movimiento de la embarcación, y una vez el jugador se ha familiarizo con otros aspectos del juego, introducir mayor influencia de las corrientes como componente del juego.

## Objetivos

La **modificación de velocidad por corrientes** y el **mapa de corrientes** pertenecen a la lógica de programación que corresponde a las mecánicas de movimiento de las unidades del juego. Desde la versión [0.031](../versions/v0-031.md)! las corrientes afectarál al barco del jugador.

Por ende, la velocidad de navegación del barco ya no vendré dada solamente por la velocidad base del barco, si no que además la corriente podrá aumentar o reducir la velocidad de la nave según la posición y el rumbo.

De esta forma se quiere que el barco del jugador se mueva un poco más deprisa cuando navegue a favor de la corriente, y un poco más despacio en caso contrario. Los parámetros que determinan el modificador de velocidad serán la posición y rotación de la embarcación, así como el tipo de barco (a mayor calado, mayor influencia de la corriente).


## Alcance de aplicación

- Carga de datos: el juego debe leer e interpretar la estructura de información deserializada para crear un mapa de corrientes
- Canvas y elementos gráficos del juego: mostrar y ocultar mapa de corrientes.
- Mapas personalizados: El jugador tiene que poder cargar un mod peronalizado con un mapa de corrientes modificado.
- Modificación en las mecánicas de movimiento: el juego debe calcular el efecto de la corriente sobre una unidad dada su transformación en el espacio.
- Ajustes de intercepción: cuando cambia el efecto de la corriente sobre la velocidad del barco del jugador, el rumbo de intercepción cuando esté persigueindo a una unidad deberá ser recalculado. Es decir, el triángulo de trayectoria será diferente si la velocidad del barco  persecutor cambia. (Ver más en [intercepción de barcos](/Documentation/game-concepts/game-concept-interception.md))


## Scripts

Las mecánicas de corrientes y generación del mapa de corrientes se gestionan principalmente desde el GameManager. Hay dos funciones principales a la hora de trabajar con los datos de corrientes:
* **createCurrentsMap**. Función encargada de crear el mapa de corrientes que utilizará el juego a partir de un archivo. La ruta del archivo puede estar en el directorio raíz de Black Flags o bien en la carpeta de un mod, según estemos jugando o no con la versión vanilla o con un mod personalizado.
* **getCurrentEffect**. Función encargada de devolver el modificdor o influencia que causa la corriente en un objeto dada su transformada. Esto es: dada su posición y rotación, se determina cuál es el factor de modificación base.

Aunque las corrientes puedan variar en el futuro por eventos aleatorios, existirá en todo momento un único mapa de corrientes con los valores base. La información de las corrientes del mapa es almacenada y accesible mediante la variable currentsMapData. Esta es una variable privada de sólo lectura, es decir, tiene una propiedad de acceso get.

```c#
    [SerializeField] private CurrentsMapData currentsMapData; //Mapa de corrientes, que se obtiene en la escena de carga.

    public CurrentsMapData CurrentsMapData
    {
        get
        {
            if(currentsMapData == null)
                createCurrentsMap();

            return currentsMapData;
        }
    }
```

El mapa de corrientes currentsMapData es un objeto perteneciente a la clase CurrentsMapData. La definición de clase se encuentra en el espacio de nombres namespace GameMechanics.Data en el archivo namespace GameMechanics.cs. Esta clase almacena los siguientes valores:

* BoundingBox: El bounding box que delimita la región sobre la que opera el mapa de corrientes. Dicho de otro modo, la caja debe estar ubicada de manera que el rango de coordenadas abarque el mapa.
* TilesX & TilesZ: El número de cuadrantes o casillas del mapa de corrientes.
* La matriz de flechas, que consiste en un array doble de flechas. Las flechas son estructuras de datos con una dirección y una intensidad de corriente.

El flujo de llamadas para obtener el mapa de corrientes es el siguiente:

```c#

    private void Awake()
    {
        //Singleton
        gm = this;
        //LoadDatabase
        WorldGenerator.Initialize();
        createCurrentsMap(); // aquí se obtiene la información de las corrientes del mapa
    //cosas...

    }

    ...

    ...

    //Game's currents map
    public void createCurrentsMap()
    {
        string path = PersistentGameSettings.currentMod == null ? Application.streamingAssetsPath : PersistentGameSettings.currentMod.ModStreaming;

#if UNITY_EDITOR
        path = "";
#endif
        path += "CurrentsMapData.json";
        print(path);

        currentsMapData = Serialization.SerializationUtils.LoadCurrentsMap(path);
    }
```

Como puede verse, podemos cargar un mapa partiendo de la ruta raíz del juego o bien utilizando el fichero de un mod en caso de cargar el juego con un mod personalizado. El mapa será obtenido deserializando el archivo json correspondiente a la ruta.

```c#
public class SerializationUtils
    {
        public static T LoadJson<T>( string path)
        {
            //Esto es sencillamente una manera abreviada de leer un json:
            T data;
            data = JsonUtility.FromJson<T>(File.ReadAllText(path));
            return data;
        }

        ...

        public static CurrentsMapData LoadCurrentsMap(string path)
        {
            var currentsMap = LoadJson<serializable_CurrentsMapData>(path);
            return new CurrentsMapData(currentsMap.boundingBox, currentsMap.tilesX, currentsMap.tilesZ, currentsMap.bufferArray);
        }

        ...
    }
```

Se observa que el programa crea una mapa de corrientes a partir de los parámetros del archivo json.
