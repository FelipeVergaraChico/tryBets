using TryBets.Bets.DTO;
using TryBets.Bets.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace TryBets.Bets.Repository;

public class BetRepository : IBetRepository
{
    protected readonly ITryBetsContext _context;
    public BetRepository(ITryBetsContext context)
    {
        _context = context;
    }

    public BetDTOResponse Post(BetDTORequest betRequest, string email)
    {
        Match ma = _context.Matches.FirstOrDefault(m => m.MatchId == betRequest.MatchId);
        if (ma == null) {
            throw new Exception("Match not founded");
        }

        User us = _context.Users.FirstOrDefault(u => u.Email == email);
        if (us == null) {
            throw new Exception("User not founded");
        }

        Team te = _context.Teams.FirstOrDefault(t => t.TeamId == betRequest.TeamId);
        if (te == null) {
            throw new Exception("Team not founded");
        }
        if (ma.MatchFinished) {
            throw new Exception("Match finished");
        }
        if (ma.MatchTeamAId != betRequest.TeamId && ma.MatchTeamBId != betRequest.TeamId) {
            throw new Exception("Team is not in this match");
        }

        Bet be = new Bet {
            UserId = us.UserId,
            MatchId = betRequest.MatchId,
            TeamId = betRequest.TeamId,
            BetValue = betRequest.BetValue
        };
        _context.Bets.Add(be);
        _context.SaveChanges();

        Bet created = _context.Bets
            .Include(b => b.Match)
            .Include(b => b.Team)
            .FirstOrDefault(b => b.BetId == be.BetId);
        if (ma.MatchTeamAId == betRequest.TeamId) {
            ma.MatchTeamAValue += betRequest.BetValue;
        } else {
            ma.MatchTeamBValue += betRequest.BetValue;
        }
        _context.Matches.Update(ma);
        _context.SaveChanges();

        return new BetDTOResponse {
            BetId = be.BetId,
            MatchId = be.MatchId,
            TeamId = be.TeamId,
            BetValue = be.BetValue,
            MatchDate = be.Match!.MatchDate,
            TeamName = be.Team!.TeamName,
            Email = be.User!.Email
        };
    }
    public BetDTOResponse Get(int BetId, string email)
    {
        Bet bet = _context.Bets
            .Include(b => b.Match)
            .Include(b => b.Team)
            .Include(b => b.User)
            .FirstOrDefault(b => b.BetId == BetId);
        if (bet == null) {
            throw new Exception("Bet not founded");
        }
        if (bet.User!.Email != email) {
            throw new Exception("User not authorized");
        }
        return new BetDTOResponse {
            BetId = bet.BetId,
            MatchId = bet.MatchId,
            TeamId = bet.TeamId,
            BetValue = bet.BetValue,
            MatchDate = bet.Match!.MatchDate,
            TeamName = bet.Team!.TeamName,
            Email = bet.User!.Email
        };
    }
}