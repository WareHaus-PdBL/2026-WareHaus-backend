public record GetAisleDto(
    int aisleNumber,
    bool isEmpty,
    int totalShelves,
    int capacity,
    int occupiedCapacity
);