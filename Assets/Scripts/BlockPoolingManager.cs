using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPoolingManager : MonoBehaviour
{
    public GameObject[] m_SquareBlocks;
    public GameObject[] m_CircleBlocks;
    public GameObject[] m_LeftTriangleBlocks;
    public GameObject[] m_RightTriangleBlocks;

    private int m_SquareIndex;
    private int m_CircleIndex;
    private int m_LeftTriangleIndex;
    private int m_RightTriangleIndex;

    void Awake()
    {
        m_SquareIndex = 0;
        m_CircleIndex = 0;
        m_LeftTriangleIndex = 0;
        m_RightTriangleIndex = 0;
    }

    public List<GameObject> GetBlocks(int count)
    {
        List<GameObject> blockList = new List<GameObject>();
        for (int i = 0; i < count; i++)
        {
            int seed = Random.Range(0, 10);
            switch (seed)
            {
                case 0:     // 70% »ç°¢Çü
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    blockList.Add(m_SquareBlocks[m_SquareIndex]);
                    m_SquareIndex = (m_SquareIndex + 1) % m_SquareBlocks.Length;
                    break;
                case 7:     // 10% Å¸¿ø
                    blockList.Add(m_CircleBlocks[m_CircleIndex]);
                    m_CircleIndex = (m_CircleIndex + 1) % m_CircleBlocks.Length;
                    break;
                case 8:     // 10% ÁÂ»ï°¢Çü
                    blockList.Add(m_LeftTriangleBlocks[m_LeftTriangleIndex]);
                    m_LeftTriangleIndex = (m_LeftTriangleIndex + 1) % m_LeftTriangleBlocks.Length;
                    break;
                case 9:     // 10% ¿ì»ï°¢Çü
                    blockList.Add(m_RightTriangleBlocks[m_RightTriangleIndex]);
                    m_RightTriangleIndex = (m_RightTriangleIndex + 1) % m_RightTriangleBlocks.Length;
                    break;
                default:
                    break;
            }
        }
        return blockList;
    }
}
