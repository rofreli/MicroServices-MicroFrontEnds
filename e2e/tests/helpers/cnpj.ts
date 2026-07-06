/**
 * Generates a random *valid* CNPJ (mod-11 check digits) so each run creates
 * unique companies and never trips the unique index on Cnpj.
 */
export function randomCnpj(): string {
  const base = Array.from({ length: 12 }, () => Math.floor(Math.random() * 10))
  // Guarantee it is not a repeated-digit sequence (those are rejected as invalid).
  base[1] = (base[0] + 1) % 10

  const digits = [...base, checkDigit(base)]
  digits.push(checkDigit(digits))

  const s = digits.join('')
  return `${s.slice(0, 2)}.${s.slice(2, 5)}.${s.slice(5, 8)}/${s.slice(8, 12)}-${s.slice(12)}`
}

function checkDigit(nums: number[]): number {
  const weights =
    nums.length === 12
      ? [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2]
      : [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2]
  const sum = nums.reduce((acc, n, i) => acc + n * weights[i], 0)
  const rest = sum % 11
  return rest < 2 ? 0 : 11 - rest
}
