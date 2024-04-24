using TryBets.Odds.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Globalization;

namespace TryBets.Odds.Repository;

public class OddRepository : IOddRepository
{
    protected readonly ITryBetsContext _context;
    public OddRepository(ITryBetsContext context)
    {
        _context = context;
    }

    public Match Patch(int MatchId, int TeamId, string BetValue)
    {
        var ma = _context.Matches.FirstOrDefault(m => m.MatchId == MatchId);
        if (ma == null) {
            throw new Exception("Match not found");
        }
        string betValueConverted = BetValue.Replace(",", ".");
        decimal oddVal = decimal.Parse(betValueConverted, CultureInfo.InvariantCulture);
        _context.Matches.Update(ma);
        if (ma.MatchTeamAId != TeamId && ma.MatchTeamBId != TeamId) {
            throw new Exception("Not Found Teams");
        }
        if (ma.MatchTeamAId == TeamId) {
            ma.MatchTeamAValue += oddVal;
        } else {
            ma.MatchTeamBValue += oddVal;
        }

        _context.SaveChanges();
        return ma;
    }
}