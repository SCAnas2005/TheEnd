using System.Collections.Generic;

public class NpcConfig
{
    public string Name { get; set; }
    public string ProfilePicturePath { get; set; }
    public Dictionary<string, AnimationConfig> Animations { get; set; }
}

public class NpcListConfig
{
    public List<NpcConfig> NPCs { get; set; }
}


public class AnimationConfig
{
    public string Path { get; set; }
    public int SpriteFrames { get; set; }
    public int FrameTime { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int LineNumber { get; set; }
    public int ColumnNumber { get; set; }
    public int LineIndex { get; set; }
    public int ColumnIndex { get; set; }
    public bool ReverseFrame { get; set; }
}
