﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ThirdTask.GameDescription
{
	public abstract class Player : Person
	{
		protected Player SecondHand { get; set; }
		protected int SplitAndSurrenderIsAllowed { get; set; } // обрезать
		protected int DoubleIsAllowed { get; set; }
		public int Bet { get; set; }
		public abstract void MakeBet();

		public override void Action(Pad pad) 
		{
			if (SumOfAllCards() == 21)
			{
				ChangeStatus("blackjack");
			}
			else
			{
				if (InputForAction == "Hit" || InputForAction == "Stand")
				{
					base.Action(pad);

					SplitAndSurrenderIsAllowed = 0;
					DoubleIsAllowed = 0;
				}
				else
				{
					if (SplitAndSurrenderIsAllowed != 0 && (/*InputForAction == "Split" ||*/ InputForAction == "Surrender"))
					{
						if (InputForAction == "Surrender")
						{
							int sum = (int)(0.5 * Bet);
							Cash += sum;
							Bet -= sum;

							ChangeStatus("surrender");
						}
						/*
						if (InputForAction == "Split")
						{
							//разделение колоды новым конструктором
							SecondHandHandler("create");

							ChangeStatus("in_game");
							SecondHand.ChangeStatus("in_game");
							// колоду для вывода надо обработать как вторую;  в идеале - черз массив (структур) в Action
						}
						*/

						SplitAndSurrenderIsAllowed = 0;
					}
					else if (DoubleIsAllowed != 0 && InputForAction == "Double")
					{
						Cash -= Bet;
						Bet *= 2;

						GetCard(pad);

						ChangeStatus("stand"); // больше игрок не ходит

						DoubleIsAllowed = 0;
						SplitAndSurrenderIsAllowed = 0;
					}

					if (PersonStatus != "surrender")
					{
						ChangeStatus();
					}
				}
			}
		}

		public override void Clear()
		{
			base.Clear();
			Bet = 0;

			SplitAndSurrenderIsAllowed = 1;
			DoubleIsAllowed = 1;
		}

		public Player() : base()
		{
			Cash = 500;
			Bet = 0;

			SecondHand = null;

			SplitAndSurrenderIsAllowed = 1;
			DoubleIsAllowed = 1;
		}

		public abstract string IsContinue();

		/*
		protected void SecondHandHandler(string prm) // обработчик второй руки
		{
			switch (prm)
			{
				case "create":

					break;
				case "destroy":

					break;
			}
		}
		*/
	}
}