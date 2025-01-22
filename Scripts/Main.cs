public class Main
{
    public GameMode Mode = GameMode.MoveMap;
    public static Main Instance = new Main();

    public CameraController MainCameraController;
    public Player MainPlayer;
}

public enum GameMode
{
    MoveMap,
    Play,
}