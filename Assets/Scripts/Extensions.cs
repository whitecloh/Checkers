using System.ComponentModel;

namespace Checkers
{
    public static class Extensions
    {
        public static Coordinate ToCoordinate(this string value)
        {
            var x = value[0];
            var y = value[2];
            return new Coordinate(x, y);
        }

        public static Coordinate ToCoordinate(this (int x, int y) value)
        {
            return new Coordinate(value.x, value.y);
        }
        
        public static string ToSerializable(this string value, PlayableSide side, RecordType recordType, string destination = "")
        {
            var playerSide = side.CurrentSide == ColorType.Black ? "1" : "2";

            switch (recordType)
            {
                case RecordType.Click:
                    return $"Player {playerSide} {recordType} to {value}";
                    
                case RecordType.Move:
                    return $"Player {playerSide} {recordType} from {value} to {destination}";
                    
                case RecordType.Remove:
                    return $"Player {playerSide} {recordType} chip at {value}";
                
                default:
                    throw new InvalidEnumArgumentException($"Action {recordType} is not supported");
            }
        }
    }
}