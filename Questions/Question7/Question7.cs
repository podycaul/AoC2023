
using Microsoft.Extensions.Configuration;

public class Question7 : QuestionBase, IQuestion
{
    public int Number => 7;

    public Question7(IConfiguration configuration) : base(configuration)
    {
        base.QuestionNumber = this.Number;
    }

    public async Task<string> Run()
    {
        var input = (await base.GetQuestionInput()).ToList();
        var hands = GetHands(input);
        var ordered = OrderByRank(hands);
        ordered.Reverse();
        double sum = 0;
        for (int i = 1; i <= ordered.Count; i++)
        {
            var hand = ordered[i - 1];
            sum += hand.Bid * i;
        }
        return sum.ToString();
    }

    private List<Hand> OrderByRank(List<Hand> hands)
    {
        List<Hand> ranked = new();
        var fiveOfKind = hands.Where(x => IsNumberOfKind(x, 5)).ToList();
        ranked.AddRange(OrderByStrength(fiveOfKind));
        fiveOfKind.ForEach(h => hands.RemoveAll(x => x.ID == h.ID));

        var fourOfKind = hands.Where(x => IsNumberOfKind(x, 4)).ToList();
        ranked.AddRange(OrderByStrength(fourOfKind));
        fourOfKind.ForEach(h => hands.RemoveAll(x => x.ID == h.ID));

        var fullHouse = hands.Where(x => IsFullHouse(x)).ToList();
        ranked.AddRange(OrderByStrength(fullHouse));
        fullHouse.ForEach(h => hands.RemoveAll(x => x.ID == h.ID));

        var threeOfKind = hands.Where(x => IsNumberOfKind(x, 3)).ToList();
        ranked.AddRange(OrderByStrength(threeOfKind));
        threeOfKind.ForEach(h => hands.RemoveAll(x => x.ID == h.ID));

        var twoPair = hands.Where(x => IsTwoPair(x)).ToList();
        ranked.AddRange(OrderByStrength(twoPair));
        twoPair.ForEach(h => hands.RemoveAll(x => x.ID == h.ID));

        var singlePair = hands.Where(x => IsNumberOfKind(x, 2)).ToList();
        ranked.AddRange(OrderByStrength(singlePair));
        singlePair.ForEach(h => hands.RemoveAll(x => x.ID == h.ID));

        var highCard = hands.Where(x => x.Cards.Distinct().Count() == 5).ToList();
        ranked.AddRange(OrderByStrength(highCard));
        highCard.ForEach(h => hands.RemoveAll(x => x.ID == h.ID));

        return ranked;
    }

    private List<Hand> OrderByStrength(List<Hand> hands)
    {
        if (hands.Count == 0) return new();
        List<Hand> ordered = new();
        Hand highestHand = hands.First();
        for (int i = 0; i < hands.Count; i++)
        {
            var hand = hands[i];
            if (highestHand.ID == hand.ID) continue;
            for (int j = 0; j < 5; j++)
            {
                if (hand.Cards[j].Strength > highestHand.Cards[j].Strength)
                {
                    highestHand = hand;
                    break;
                }
                else if (hand.Cards[j].Strength < highestHand.Cards[j].Strength) break;
                else if (hand.Cards[j].Strength == highestHand.Cards[j].Strength)
                {
                    // found collision
                    for (int k = j + 1; k < 5; k++)
                    {
                        if (hand.Cards[k].Strength > highestHand.Cards[k].Strength)
                        {
                            highestHand = hand;
                            break;
                        }
                        else if (hand.Cards[k].Strength < highestHand.Cards[k].Strength)
                        {
                            // highest hand stays the same
                            break;
                        }
                    }
                }
            }
        }
        ordered.Add(highestHand);
        ordered.AddRange(OrderByStrength(hands.Where(x => x.ID != highestHand.ID).ToList()));
        return ordered;
    }

    private bool IsFullHouse(Hand hand)
    {
        foreach (var card in hand.Cards)
        {
            if (hand.Cards.Count(c => c.Strength == card.Strength) == 3)
            {
                var remaining = hand.Cards.Where(c => c.Strength != card.Strength);
                return remaining.Count(x => x.Strength == remaining.First().Strength) == 2;
            }

        }

        return false;
    }

    private bool IsTwoPair(Hand hand)
    {
        int pairs = 0;
        HashSet<int> counted = new();
        foreach (var card in hand.Cards)
        {
            if (counted.Contains(card.Strength)) continue;
            if (hand.Cards.Count(x => x.Strength == card.Strength) == 2) pairs++;
            counted.Add(card.Strength);
        }

        return pairs == 2;
    }

    private bool IsNumberOfKind(Hand hand, int number)
    {
        foreach (var card in hand.Cards)
        {
            if (hand.Cards.Count(x => x.Strength == card.Strength) == number) return true;
        }
        return false;
    }

    private List<Hand> GetHands(List<string> input)
    {
        List<Hand> hands = new();
        foreach (var line in input)
        {
            string[] handInput = line.Split(" ");

            Hand hand = new()
            {
                Bid = int.Parse(handInput[1].Trim())
            };
            string cards = handInput[0].Trim();
            foreach (var cardInput in cards)
            {
                Card card = new();
                if (char.IsNumber(cardInput)) card.Number = int.Parse(cardInput.ToString());
                else card.Face = Enum.Parse<Face>(cardInput.ToString());
                hand.Cards.Add(card);
            }
            hands.Add(hand);
        }
        return hands;
    }

    private record Hand
    {
        public Guid ID { get; } = Guid.NewGuid();
        public double Bid { get; init; }
        public List<Card> Cards { get; init; } = new();
    }

    private record Card
    {
        public int Strength => Face != Face.Unset ? (int)Face : Number;
        public Face Face { get; set; } = Face.Unset;
        public int Number { get; set; }
    }

    private enum Face
    {
        Unset = 0,
        A = 14,
        K = 13,
        Q = 12,
        J = 11,
        T = 10
    }
}