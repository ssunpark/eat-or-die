using UnityEngine;

public class CharacterCustomizer : MonoBehaviour
{
    [System.Serializable]
    public class PartSet
    {
        public string PartName;
        public GameObject[] Options;
        [HideInInspector] public int CurrentIndex = 0;

        public void ShowOption(int index)
        {
            for (int i = 0; i < Options.Length; i++)
            {
                Options[i].SetActive(i == index);
            }
        }

        public void Next()
        {
            CurrentIndex = (CurrentIndex + 1) % Options.Length;
            ShowOption(CurrentIndex);
        }

        public void Prev()
        {
            CurrentIndex = (CurrentIndex - 1 + Options.Length) % Options.Length;
            ShowOption(CurrentIndex);
        }
    }

    public PartSet[] Parts;

    private void Start()
    {
        foreach (var part in Parts)
        {
            part.ShowOption(part.CurrentIndex);
        }
    }

    public void NextPart(string partName)
    {
        var part = System.Array.Find(Parts, p => p.PartName == partName);
        part?.Next();
    }

    public void PrevPart(string partName)
    {
        var part = System.Array.Find(Parts, p => p.PartName == partName);
        part?.Prev();
    }

    public int GetActualIndex(string partName)
    {
        var part = System.Array.Find(Parts, p => p.PartName == partName);
        if (part == null || part.Options.Length == 0)
            return 0;

        var selected = part.Options[part.CurrentIndex];
        if (selected == null) return 0;

        string[] split = selected.name.Split('_');
        if (split.Length < 2) return 0;

        if (int.TryParse(split[1], out int result))
            return result;

        return 0;
    }

}
