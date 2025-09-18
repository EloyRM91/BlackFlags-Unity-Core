using GameMechanics.save;

using GameSettings.Core;

/// <summary>
/// Clase que contiene temporalmente los datos de la partida cargada,
/// y que luego se destruye una vez se han iniicalizado todos los compoenntes de escena
/// </summary>
public class PersistentSavedFileContainer : Persistent
{
    //Inventory - Inventario y armamento
    public bool ignoreInventoryData = false;
    private bool inventoyDataLoaded;

    //Player Data - Transformación y datos de ruta y destino
    public bool ignorePlayerConvoyData = false;
    private bool playerConvoyDataLoaded;

    //Morale - Tripulación, racionamiento y modificadores
    public bool ignoreMoraleData = false;
    private bool moraleDataLoaded;

    //Calendary - Fecha, timer de día y de actualización de mercados
    public bool ignoreDateData = false;
    private bool dateDataLoaded;

    //public delegate void OnImportantDataLoaded();
    //public OnImportantDataLoaded onImportantDataLoaded;

    public SavedFile savedFile;

    protected override void Awake()
    {
        base.Awake();
        //onImportantDataLoaded = CheckData;
    }

    //public void CheckData()
    public void OnImportantDataLoaded()
    {
        if (
            (ignoreInventoryData || inventoyDataLoaded) &&
            (ignorePlayerConvoyData || playerConvoyDataLoaded) &&
            (ignoreMoraleData || moraleDataLoaded) &&
            (ignoreDateData || dateDataLoaded)
            )
        {
            //Hemos cargado todo. Ahorramos memoria borrando todo y destruyendo este componente
            savedFile = null;
            PersistentGameSettings.loadingFile = false;
            PersistentGameSettings.selectedFileName = null;
            Destroy(this);
        }
    }

    public void OnLoadInventory()
    {
        inventoyDataLoaded = true;
        OnImportantDataLoaded();
    }

    public void OnLoadPlayerRoute()
    {
        playerConvoyDataLoaded = true;
        OnImportantDataLoaded();
    }

    public void OnLoadMorale()
    {
        moraleDataLoaded = true;
        OnImportantDataLoaded();
    }

    public void OnLoadedCalendar()
    {
        dateDataLoaded = true;
        OnImportantDataLoaded();
    }

    public void OnIgnoreInventoryData()
    {
        ignoreInventoryData = true;
        OnImportantDataLoaded();
    }

    public void OnIgnorePlayerRoute()
    {
        ignorePlayerConvoyData = true;
        OnImportantDataLoaded();
    }

    public void OnIgnoreMoraleData()
    {
        ignoreMoraleData = true;
        OnImportantDataLoaded();
    }

    public void OnIgnoreCalendarData()
    {
        ignoreDateData = true;
        OnImportantDataLoaded();
    }
}