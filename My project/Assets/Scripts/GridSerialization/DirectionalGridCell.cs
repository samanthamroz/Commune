[System.Serializable]
public struct DirectionalGridCell {
    public int cellValue;
    public Direction directionsOpen;

    public DirectionalGridCell(int cellValue, Direction directionsOpen) {
        this.cellValue = cellValue;
        this.directionsOpen = directionsOpen;
    }
}