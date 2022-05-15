using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadThing : Thing
{
    public GameObject[] SwapableGameobjects;

    private PlayerController.Controls _originalPlayer;
    private int _currentSwapableIndex;

    public void Init(PlayerController.Controls originalPlayer, 
        Vector2 startingVelocity = default)
    {
        _currentSwapableIndex = Random.Range(0, SwapableGameobjects.Length);
        _originalPlayer = originalPlayer;
        SwapableGameobjects[_currentSwapableIndex].SetActive(true);

        base.Init(startingVelocity, SwapableGameobjects[_currentSwapableIndex].GetComponent<SpriteRenderer>());
    }

    public void Swap()
    {
        //switch (_originalPlayer)
        //{
        //    case PlayerController.Controls.WASD:
        //        if (transform.position.x )
        //        break;
        //    case PlayerController.Controls.Arrow:
        //        break;
        //    default:
        //        break;
        //}
        SwapableGameobjects[_currentSwapableIndex].SetActive(false);
        ++_currentSwapableIndex;
        _currentSwapableIndex = _currentSwapableIndex % SwapableGameobjects.Length;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
