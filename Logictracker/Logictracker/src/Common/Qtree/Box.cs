namespace Logictracker.Qtree
{
    public class Box
    {
        public long Top { get; set; }
        public long Bottom { get; set; }
        public long Left { get; set; }
        public long Right { get; set; }
        public Box(long top, long bottom, long left, long right)
        {
            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
        }
        public Box(double top, double bottom, double left, double right, BaseQtree qtree)
        {
            var newTl = qtree.GetIndex(top, left);
            var newBr = qtree.GetIndex(bottom, right);
            Top = newTl.Y+1;
            Bottom = newBr.Y;
            Left = newTl.X;
            Right = newBr.X+1;
        }
    }
}
