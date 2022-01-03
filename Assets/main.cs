using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class main : MonoBehaviour
{

    private int activeRow, possibleSols, removed, index = -1;
    private bool pressing;

    private List<int[]> remainingCombos;

    public Text correctText, closeText, remainingText, removedText, posText, imposText;

    public Button submit, reset, prev, rand, next, quit;
    public Button[] row1, row2, row3, row4, row5, row6, row7, row8, row9, row10, redButtons, whiteButtons;
    private Button[][] pegs;

    public SpriteRenderer[] srow1, srow2, srow3, srow4, srow5, srow6, srow7, srow8, srow9, srow10, boxes, disp;
    private SpriteRenderer[][] inds;

    private Color[] colors;
    private Color[] indsColors; 
    private int[,] currentColors;
    private int[] reds, whites;

    // Start is called before the first frame update
    void Start()
    {
        init();
    }

    void init()
    {
        index = -1;
        closeText.gameObject.SetActive(false);
        correctText.gameObject.SetActive(false);
        posText.gameObject.SetActive(false);
        imposText.gameObject.SetActive(false);
        pegs = new[] { row1, row2, row3, row4, row5, row6, row7, row8, row9, row10 };
        inds = new[] { srow1, srow2, srow3, srow4, srow5, srow6, srow7, srow8, srow9, srow10 };
        reds = new int[10];
        whites = new int[10];
        for (var i = 0; i < 10; i++)
        {
            reds[i] = 0;
            whites[i] = 0;
        }
        pressing = true;
        activeRow = 0;
        renderBox();
        setupColors();
        foreach (var s in disp)
        {
            s.color = colors[6];
        }
        setupButtons();
        setupPossibilities();

    }

    void setupColors() 
    {
        colors = new[] { Color.white, Color.cyan, Color.yellow, Color.magenta, Color.green, Color.blue, Color.gray };
        indsColors = new[] { Color.black, Color.white, new Color(180,0,0,255)};
    }

    void setupButtons()
    {
        currentColors = new int[10,4];
        for (var i = 0; i < 10; i++)
        {
            for (var j = 0; j < 4; j++)
            {
                int r = i, p = j;
                pegs[i][j].GetComponentInParent<Image>().color = Color.white;
                var c = pegs[i][j].colors;
                c.normalColor = colors[6];
                c.highlightedColor = colors[6];
                c.selectedColor = colors[6];
                c.pressedColor = colors[6];
                pegs[i][j].colors = c;
                pegs[i][j].onClick.RemoveAllListeners();
                pegs[i][j].onClick.AddListener(delegate { buttonPress(r, p); });
                currentColors[i, j] = 6;

                inds[i][j].color = indsColors[0];
            }
        }
        submit.onClick.RemoveAllListeners();
        reset.onClick.RemoveAllListeners();
        submit.onClick.AddListener(submitPress);
        reset.onClick.AddListener(init);
        foreach (var b in redButtons)
        {
            b.gameObject.SetActive(false);
        }
        foreach (var b in whiteButtons)
        {
            b.gameObject.SetActive(false);
        }
        for (var i = 0; i < 5; i++)
        {
            var c = i;
            redButtons[i].onClick.RemoveAllListeners();
            whiteButtons[i].onClick.RemoveAllListeners();
            redButtons[i].onClick.AddListener(delegate { redButtonPress(c); });
            whiteButtons[i].onClick.AddListener(delegate { whiteButtonPress(c); });
        }

        prev.onClick.RemoveAllListeners();
        prev.onClick.AddListener(dispPrev);
        rand.onClick.RemoveAllListeners();
        rand.onClick.AddListener(dispRand);
        next.onClick.RemoveAllListeners();
        next.onClick.AddListener(dispNext);

        quit.onClick.RemoveAllListeners();
        quit.onClick.AddListener(exit);
    }

    void setupPossibilities()
    {
        remainingCombos = new List<int[]>();
        for (var i = 0; i < 6; i++)
        {
            for (var j = 0; j < 6; j++)
            {
                for (var k = 0; k < 6; k++)
                {
                    for (var l = 0; l < 6; l++)
                    {
                        remainingCombos.Add(new[]{i,j,k,l});
                    }
                }
            }
        }
        possibleSols = remainingCombos.Count;
        removed = 0;
        Debug.Log("Remaining Possible Solutions: " + possibleSols);
        renderCount();
    }

    void exit()
    {
        Application.Quit();
    }

    void dispCombo(int[] c)
    {
        for (var i = 0; i < 4; i++)
        {
            disp[i].color = colors[c[i]];
        }
    }

    void dispPrev()
    {
        var t = index;
        var max = remainingCombos.Count;
        if (index > max - 1 || index <= 0)
        {
            index = max-1;
        }
        else
        {
            index--;
        }
        Debug.Log("DispPrev called. Index before: " + t + ". Index after: " + index + ".");
        dispCombo(remainingCombos[index]);
    }

    void dispRand()
    {
        var t = index;
        var max = remainingCombos.Count;
        index = Random.Range(0, max);
        Debug.Log("DispRand called. Index before: " + t + ". Index after: " + index + ".");
        dispCombo(remainingCombos[index]);
    }

    void dispNext()
    {
        var t = index;
        var max = remainingCombos.Count;
        if (index >= max - 1 || index < 0)
        {
            index = 0;
        }
        else
        {
            index++;
        }
        Debug.Log("DispNext called. Index before: " + t + ". Index after: " + index + ".");
        dispCombo(remainingCombos[index]);
    }

    void submitPress()
    {
        if (!pressing) return;
        for (var i = 0; i < 4; i++)
        {
            if (currentColors[activeRow,i] == 6)
            {
                return;
            }
        }

        if (activeRow > 0)
        {
            for (var i = 0; i < activeRow; i++)
            {
                if (currentColors[activeRow,0] == currentColors[i,0] && currentColors[activeRow, 1] == currentColors[i, 1] && currentColors[activeRow, 2] == currentColors[i, 2] && currentColors[activeRow, 3] == currentColors[i, 3])
                {
                    return;
                }
            }
        }
        pressing = false;
        foreach (var s in disp)
        {
            s.color = colors[6];
        }

        sayIfPossible();

        startHinting();

    }

    void sayIfPossible()
    {
        posText.gameObject.SetActive(false);
        imposText.gameObject.SetActive(false);
        var sub = new[] { currentColors[activeRow, 0], currentColors[activeRow, 1], currentColors[activeRow, 2], currentColors[activeRow, 3] };
        var s = sub[0].ToString() + sub[1].ToString() + sub[2].ToString() + sub[3].ToString();
        var y = remainingCombos.Any(v => s.Equals(v[0].ToString() + v[1].ToString() + v[2].ToString() + v[3].ToString()));
        if (y)
        {
            posText.gameObject.SetActive(true);
        }
        else
        {
            imposText.gameObject.SetActive(true);
        }
        
    }

    void startHinting()
    {
        foreach (var b in redButtons)
        {
            b.gameObject.SetActive(true);
        }
        correctText.gameObject.SetActive(true);
    }

    void redButtonPress(int c)
    {
        reds[activeRow] = c;
        if (reds[activeRow] > 0)
        {
            for (var i = 0; i < reds[activeRow]; i++)
            {
                inds[activeRow][i].color = indsColors[2];
            }
        }
        foreach (var b in redButtons)
        {
            b.gameObject.SetActive(false);
        }
        correctText.gameObject.SetActive(false);
        if (reds[activeRow] == 4)
        {
            updatePossibilities();
            boxes[activeRow].gameObject.SetActive(false);
            Debug.Log("That's correct!");
            return;
        }
        for (var i = 0; i < 5 - reds[activeRow]; i++)
        {
            whiteButtons[i].gameObject.SetActive(true);
        }
        closeText.gameObject.SetActive(true);
    }

    void whiteButtonPress(int c)
    {
        whites[activeRow] = c;
        if (reds[activeRow] + whites[activeRow] > 4)
        {
            Debug.Log("Total indicators = " + (reds[activeRow] + whites[activeRow]) + "! Returning.");
            return;
        }
        if (whites[activeRow] > 0)
        {
            for (var i = reds[activeRow]; i < reds[activeRow]+whites[activeRow]; i++)
            {
                inds[activeRow][i].color = indsColors[1];
            }
        }
        foreach (var b in whiteButtons)
        {
            b.gameObject.SetActive(false);
        }
        closeText.gameObject.SetActive(false);
        updatePossibilities();
        if (activeRow < 9)
        {
            activeRow++;
            renderBox();
        }
        else
        {
            Debug.Log("out of rows");
            return;
        }
        pressing = true;
    }

    void renderBox()
    {
        foreach (var b in boxes)
        {
            b.gameObject.SetActive(false);
        }
        boxes[activeRow].gameObject.SetActive(true);
    }

    void renderCount()
    {
        remainingText.text = possibleSols.ToString();
        removedText.text = removed.ToString();
    }

    void buttonPress(int row, int pos)
    {
        if (!pressing || activeRow != row)
        {
            return;
        }
        var initialColor = currentColors[row, pos];
        int nextColor;
        if (initialColor == 5 || initialColor == 6)
        {
            nextColor = 0;
        }
        else
        {
            nextColor = initialColor + 1;
        }
        currentColors[row, pos] = nextColor;
        var c = pegs[row][pos].colors;
        c.normalColor = colors[nextColor];
        c.highlightedColor = colors[nextColor];
        c.selectedColor = colors[nextColor];
        c.pressedColor = colors[nextColor];
        pegs[row][pos].colors = c;
    }
    
    void updatePossibilities()
    {
        var a = activeRow;
        var r = reds[a];
        var w = whites[a];
        var toRemove = new List<int[]>();
        removed = 0;
        var co = 0;
        foreach (var c in remainingCombos)
        {
            co++;
            var same = 0;
            var sim = 0;
            var col = new [] {c[0], c[1] , c[2] , c[3] };
            var row = new[] { currentColors[a, 0], currentColors[a, 1], currentColors[a, 2], currentColors[a, 3] };
            for (var i = 0; i < 4; i++)
            {
                if (col[i] == row[i])
                {
                    same++;
                    col[i] = -1;
                    row[i] = -2;
                }

            }

            var f = new[] { 0, 0, 0, 0, 0, 0 };
            var s = new[] { 0, 0, 0, 0, 0, 0 };

            foreach (var v in row)
            {
                if (v != -2)
                {
                    f[v]++;
                }
            }
            foreach (var v in col)
            {
                if (v != -1)
                {
                    s[v]++;
                }
            }

            for (var i = 0; i < 6; i++)
            {
                if (f[i] != 0 && s[i] != 0)
                {
                    if (s[i]<=f[i])
                    {
                        sim+=s[i];
                    }
                    else
                    {
                        sim += f[i];
                    }
                }
            }

            if (sim != w || same != r)
            {
                toRemove.Add(c);
            }

        }
        foreach (var c in toRemove)
        {
            remainingCombos.Remove(c);
            removed++;
        }
        Debug.Log("Removed " + removed + " possible answers.");
        possibleSols = remainingCombos.Count;
        Debug.Log("Remaining Possible Solutions: " + possibleSols);
        if (possibleSols == 1)
        {
            dispCombo(remainingCombos[0]);
        }
        renderCount();
    }
}