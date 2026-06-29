using BusinessUnits.Domain.Exceptions;

namespace BusinessUnits.Domain.ValueObjects;

public sealed class Cnpj : IEquatable<Cnpj>
{
    public string Value { get; }

    public Cnpj(string value)
    {
        var digits = Clean(value);
        if (!IsValid(digits))
            throw new DomainException($"CNPJ '{value}' is invalid.");
        Value = Format(digits);
    }

    private static string Clean(string cnpj) =>
        new(cnpj.Where(char.IsDigit).ToArray());

    private static string Format(string digits) =>
        $"{digits[..2]}.{digits[2..5]}.{digits[5..8]}/{digits[8..12]}-{digits[12..]}";

    public static bool IsValid(string cnpj)
    {
        if (cnpj.Length != 14) return false;
        if (cnpj.Distinct().Count() == 1) return false;

        int[] w1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] w2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        int Calc(int[] weights, int length)
        {
            var sum = weights.Select((w, i) => (cnpj[i] - '0') * w).Sum();
            var rem = sum % 11;
            return rem < 2 ? 0 : 11 - rem;
        }

        return cnpj[12] - '0' == Calc(w1, 12) && cnpj[13] - '0' == Calc(w2, 13);
    }

    public bool Equals(Cnpj? other) => other is not null && Value == other.Value;
    public override bool Equals(object? obj) => Equals(obj as Cnpj);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value;
}
