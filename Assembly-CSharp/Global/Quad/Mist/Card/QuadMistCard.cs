﻿using System;
using System.Collections.Generic;
using System.IO;
using FF9;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;
using Memoria.Prime.PsdFile;
using UnityEngine;
using Object = System.Object;

public class QuadMistCard
{
	public QuadMistCard()
	{
	}

	public QuadMistCard(QuadMistCard card)
	{
		id = card.id;
		side = card.side;
		atk = card.atk;
		type = card.type;
		pdef = card.pdef;
		mdef = card.mdef;
		cpoint = card.cpoint;
		arrow = card.arrow;
    }

    public void LevelUpInMatch()
	{
		switch (UnityEngine.Random.Range(0, 3))
		{
		case 0:
			if (atk != CardPool.GetMaxStatCard((Int32)id).atk)
			{
				atk = (Byte)(atk + 1);
			}
			break;
		case 1:
			if (pdef != CardPool.GetMaxStatCard((Int32)id).pdef)
			{
				pdef = (Byte)(pdef + 1);
			}
			break;
		case 2:
			if (mdef != CardPool.GetMaxStatCard((Int32)id).mdef)
			{
				mdef = (Byte)(mdef + 1);
			}
			break;
		}
	}

	public void LevelUpInBattle()
	{
		switch (type)
		{
		case QuadMistCard.Type.PHYSICAL:
			if (UnityEngine.Random.Range(0, 64) == 0)
			{
				type = QuadMistCard.Type.FLEXIABLE;
			}
			break;
		case QuadMistCard.Type.MAGIC:
			if (UnityEngine.Random.Range(0, 64) == 0)
			{
				type = QuadMistCard.Type.FLEXIABLE;
			}
			break;
		case QuadMistCard.Type.FLEXIABLE:
			if (UnityEngine.Random.Range(0, 128) == 0)
			{
				type = QuadMistCard.Type.ASSAULT;
			}
			break;
		}
	}

	public override String ToString()
	{
		if (Configuration.Mod.TranceSeek || (Configuration.TetraMaster.TripleTriad > 0))
		{
            if (IsBlock)
            {
                char c = '1';
                return string.Concat(new object[]
                {
                atk.ToString("X").ToLower()[0],
                c.ToString(),
                pdef.ToString("X").ToLower()[0],
                mdef.ToString("X").ToLower()[0]
                });
            }
            TripleTriadCard baseCard = TripleTriad.TripleTriadCardStats[(TripleTriadId)id];
			return string.Concat(new object[]
            {
            baseCard.atk.ToString("X").ToLower()[0],
            baseCard.matk.ToString("X").ToLower()[0],
            baseCard.pdef.ToString("X").ToLower()[0],
            baseCard.mdef.ToString("X").ToLower()[0]
            });
        }
		else
		{
            Char c = 'p';
            if (type == QuadMistCard.Type.MAGIC)
            {
                c = 'm';
            }
            if (type == QuadMistCard.Type.FLEXIABLE)
            {
                c = 'x';
            }
            if (type == QuadMistCard.Type.ASSAULT)
            {
                c = 'a';
            }
            return String.Concat(new Object[]
            {
            (atk >> 4).ToString("X").ToLower()[0],
            c.ToString(),
            (pdef >> 4).ToString("X").ToLower()[0],
            (mdef >> 4).ToString("X").ToLower()[0]
            });
        }
	}

	public Int32 ArrowNumber => MathEx.BitCount(arrow);

	public Boolean IsBlock
	{
		get
		{
			return id >= 100;
		}
	}

	public Boolean isTheSameCard(QuadMistCard card)
	{
		global::Debug.Log(String.Concat(new Object[]
		{
			"isTheSameCard 1 current card: id = ",
			id,
			", atk = ",
			atk,
			", arrow = ",
			arrow,
			", type = ",
			type,
			", pdef = ",
			pdef,
			", mdef = ",
			mdef
		}));
		global::Debug.Log(String.Concat(new Object[]
		{
			"isTheSameCard 2 taken   card: id = ",
			card.id,
			", atk = ",
			card.atk,
			", arrow = ",
			card.arrow,
			", type = ",
			card.type,
			", pdef = ",
			card.pdef,
			", mdef = ",
			card.mdef
		}));
		if (id == card.id && atk == card.atk && arrow == card.arrow && type == card.type && pdef == card.pdef && mdef == card.mdef)
		{
			global::Debug.Log("isTheSameCard 3 return true");
			return true;
		}
		global::Debug.Log("isTheSameCard 4 return false");
		return false;
	}

	public Byte id;

	public Byte side;

	public Byte atk;

	public QuadMistCard.Type type;

	public Byte pdef;

	public Byte mdef;

	public Byte cpoint;

	public Byte arrow;

	public enum Type
	{
		PHYSICAL,
		MAGIC,
		FLEXIABLE,
		ASSAULT
	}
}
