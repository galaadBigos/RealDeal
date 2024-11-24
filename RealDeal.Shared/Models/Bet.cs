﻿using RealDeal.Shared.Enums;

namespace RealDeal.Shared.Models;

public class Bet : ModelBase
{
	public BetType Type { get; set; }
	public BetStatus Status { get; set; }
	public float Amount { get; set; }
	public float Rating { get; set; }
	public float Gain { get; set; }
	public Guid BettorId { get; set; }
	public Guid MatchId { get; set; }
	public DateTime CreatedAt { get; set; }
}
