namespace CSharpExt;

public record class Recurrence(int Times, double Delay, IList<int> ErrorCodes) {
   public Recurrence() : this(Times: 0, Delay: 0, ErrorCodes: []) { }
}