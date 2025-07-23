## 아이템 클래스 다이어그램

```mermaid
classDiagram
	class Item {
    	<<abstract>>
		+Data: ItemData
		-data: ItemData
		+Item(ItemData data) Item
	}

	class ItemData {
		+Name: string
		+Image: Sprite
		+Prefab: GameObject
	}

	class ICarryable {
    	<<interface>>
	}
    
    class MaterialItem {
        +ToolNeeded: SmithingTool 
        +TimeOrCountToRefine: float 
    }

    class MaterialItemData {
        +MaterialType: MaterialType
    }

    class MaterialType {
        <<enumeration>>
    }

    class CompletedMaterialItem {
        +UsedCraft: List~CraftData~
    }
    
    class CraftData {
        +Product: Item 
        +Materials: List~CompletedMaterialItem~ 
    }

    class ProductItem {

    }

	ItemData "1" o-- "1" Item: has
    ScriptableObject <|-- ItemData
    Item <|-- MaterialItem: inherit
    ItemData <|-- MaterialItemData: inherit
    MaterialItemData "1" o-- "1" MaterialItem: has
    MaterialType "1" o-- "1" MaterialItemData: has
    Item <|-- CompletedMaterialItem: inherit
    Item <|-- ProductItem : inherit
    CraftData "*" o-- CompletedMaterialItem: has
    ICarryable <|.. Item: implement
```

## Tool 클래스 다이어그램


```mermaid
classDiagram
	class SmithingTool {
    	<<abstract>>
        +bool IsFinished
        +AllowedItemTypes: List~ItemType~
        +HoldingItem: Nullable~MaterialItem~  
        +RemainingTime: float 
        +ShowTimerUI
        +ShowItemIconUI
	}

    class WoodWorkTable {
        +HasCraftableItem(Player): bool
        +ShowPrductIconUI()
        -CraftList: List~CraftData~
    }

    class Anvil {

    }

    class CraftTable {
        
    }
    
    MaterialItem "1" o-- SmithingTool: has
    ItemType "1" o-- SmithingTool: has
    SmithingTool <|-- WoodWorkTable: inherit
    SmithingTool <|-- Anvil: inherit 
    SmithingTool <|-- CraftTable: inherit
    CraftData "*" o-- WoodWorkTable: has
```
