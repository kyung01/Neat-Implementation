using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TicTacToeRenderer : MonoBehaviour
{
	[SerializeField]
	List<Text> slots;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	public void render(TicTacToeBoard board)
	{
		for(int i = 0; i < 9; i++)
		{
			switch (board.slots[i]) {
				case TicTacToeBoard.SLOT_STATUS.EMPTY:
					slots[i].text = "";
					break;
				case TicTacToeBoard.SLOT_STATUS.O:
					slots[i].text = "O";
					break;
				case TicTacToeBoard.SLOT_STATUS.X:
					slots[i].text = "X";
					break;
			}

		}
	}

}
