<h4 align="center">
:construction: Proyecto en construcción en el que trabajaré en mis ratos libres :) :construction:
</h4>

# SimpleVRFileExplorer

Explorador de archivos para realidad virtual y modo 2D.

Modo 2D

![Explorador de archivos modo 2D](https://raw.githubusercontent.com/NocSchecter/UnityFileBrowser/develop/screens/2DTest.gif)

Modo VR

![Explorador de archivos modo VR](https://raw.githubusercontent.com/NocSchecter/UnityFileBrowser/develop/screens/VRTest.gif)

# Característica

1. Navegar por los directorios y archivos del sistema de archivos.
2. Seleccionar un archivo y recibir una notificación del evento de selección a través de un evento
3. personalizar la lista de extensiones de archivo
4. Modo VR (Oculus Quest) Y 2D
5. Interfaz sencilla de usar
6. Reproductor de video (Su interfaz está muy sosa, estoy trabajando en eso)

# Componentes necesarios para su funcionamiento:

1. Unity 2020.3.12f1 o superior
2. Oculus XR Plugin 1.11.2
3. TextmeshPro 3.0.6
4. Universal RP 10.5.1
5. XR Plugin Management 4.3.1
6. Oculus SDK para Quest

# Como usar

Hay 2 escenas llamadas 2D-FileBrowser y VR-FileBrowser que funcionan para modo desktop y VR.

Para modo 2D basta con abrir la escena y darle play para poder interactuar con el ejemplo.
Para el modo VR es necesario Instalar el Oculus integration, que se puede encontrar en el asset store. https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022

Si quieres implementar una nueva característica, como por ejemplo reproducir archivos mp3, en tu script suscríbete al evento FileBrowser.FileSelected que está en el script FileBrowser.

Ejemplo de cómo suscribirte desde otro script:

```
    //Se suscribe al evento
    private void Start()
    {
        FileBrowser.FileSelected += OnFileSelected;
    }
    
    //Asigna la ruta del archivo a la variable newPath
    private void OnFileSelected(string path)
    {
        newPath = path;
    }
```

# Funciones que se agregaran mas adelante

1. Controles y mejor interfaz para el reproductor de video
2. Reproducir video Plano y 360
3. Playlist (Crear, añadir, eliminar, guardar y cargar)
4. Soporte para el sistema de ficheros de Android
5. Ejemplos para cargar archivos de audio, imágenes, texto, etc.
6. Diseño mejorado tomando en cuenta la experiencia de usuario

