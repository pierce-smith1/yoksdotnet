namespace yoksdotnet.common;

public static class Ids
{
    private static long _runningId = 0;

    public static long NewId() => _runningId++;
}
