namespace SlotMachineAPI.Domain.Entities
{
    public class SpinResult
    {
        public int[][] Matrix { get; set; }
        public decimal WinAmount { get; set; }
        public decimal CurrentBalance { get; set; }
    }
}
