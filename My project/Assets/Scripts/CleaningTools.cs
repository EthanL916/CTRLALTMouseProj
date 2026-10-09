using UnityEngine;

public enum ToolType
{
    Sponge,
    Rag,
    GlassCleaner,
    Duster
}

[System.Serializable]
public class CleaningTool
{
    public string toolName;
    public ToolType type;
    public Sprite toolSprite;
    public Sprite activeSprite;
}