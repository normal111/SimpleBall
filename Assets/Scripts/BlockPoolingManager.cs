using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPoolingManager : MonoBehaviour
{
    public int m_PoolingCount = 42;
    public GameObject m_SquareBlockPrefab;
    public GameObject m_LeftTriangleBlockPrefab;
    public GameObject m_RightTriangleBlockPrefab;
    public GameObject m_EllipseBlockPrefab;

    private Block[] m_SquareBlocks;
    private Block[] m_LeftTriangleBlocks;
    private Block[] m_RightTriangleBlocks;
    private Block[] m_EllipseBlocks;

    private int m_SquareIndex;
    private int m_LeftTriangleIndex;
    private int m_RightTriangleIndex;
    private int m_EllipseIndex;

    void Awake()
    {
        m_SquareBlocks = new Block[m_PoolingCount];
        m_LeftTriangleBlocks = new Block[m_PoolingCount];
        m_RightTriangleBlocks = new Block[m_PoolingCount];
        m_EllipseBlocks = new Block[m_PoolingCount];

        for (int i = 0; i < m_PoolingCount; i++)
        {
            m_SquareBlocks[i] = Instantiate(m_SquareBlockPrefab, this.transform).GetComponent<Block>();
            m_LeftTriangleBlocks[i] = Instantiate(m_LeftTriangleBlockPrefab, this.transform).GetComponent<Block>();
            m_RightTriangleBlocks[i] = Instantiate(m_RightTriangleBlockPrefab, this.transform).GetComponent<Block>();
            m_EllipseBlocks[i] = Instantiate(m_EllipseBlockPrefab, this.transform).GetComponent<Block>();
        }

        m_SquareIndex = 0;
        m_LeftTriangleIndex = 0;
        m_RightTriangleIndex = 0;
        m_EllipseIndex = 0;
    }

    public List<Block> GetBlocks(int count)
    {
        List<Block> blockList = new List<Block>();
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

                case 7:      // 10% ÁÂ»ï°¢Çü
                    blockList.Add(m_LeftTriangleBlocks[m_LeftTriangleIndex]);
                    m_LeftTriangleIndex = (m_LeftTriangleIndex + 1) % m_LeftTriangleBlocks.Length;
                    break;

                case 8:     // 10% ¿ì»ï°¢Çü
                    blockList.Add(m_RightTriangleBlocks[m_RightTriangleIndex]);
                    m_RightTriangleIndex = (m_RightTriangleIndex + 1) % m_RightTriangleBlocks.Length;
                    break;

                case 9:     // 10% Å¸¿ø
                    blockList.Add(m_EllipseBlocks[m_EllipseIndex]);
                    m_EllipseIndex = (m_EllipseIndex + 1) % m_EllipseBlocks.Length;
                    break;

                default:
                    break;
            }
        }
        return blockList;
    }

    public Block GetBlock(Block.BlockType blockType)
    {
        Block block = null;
        switch (blockType)
        {
            case Block.BlockType.Square:
                block = m_SquareBlocks[m_SquareIndex];
                m_SquareIndex = (m_SquareIndex + 1) % m_SquareBlocks.Length;
                break;

            case Block.BlockType.LeftTri:
                block = m_LeftTriangleBlocks[m_LeftTriangleIndex];
                m_LeftTriangleIndex = (m_LeftTriangleIndex + 1) % m_LeftTriangleBlocks.Length;
                break;

            case Block.BlockType.RightTri:
                block = m_RightTriangleBlocks[m_RightTriangleIndex];
                m_RightTriangleIndex = (m_RightTriangleIndex + 1) % m_RightTriangleBlocks.Length;
                break;

            case Block.BlockType.Ellipse:
                block = m_EllipseBlocks[m_EllipseIndex];
                m_EllipseIndex = (m_EllipseIndex + 1) % m_EllipseBlocks.Length;
                break;
        }
        return block;
    }
}
