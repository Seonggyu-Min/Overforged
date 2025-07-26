## 도구 Network 클래스

```mermaid
classDiagram
    class INetSynchronizable {
        +PlayerNetworkId: int
        +IsOwner: bool
        +SceneId: int
        +OnRpc(method: string, latency: float, args: Array~object~)
        -SendRpc(method: string, args: Array~object~)
    }

    class NetworkSyncrhonizer {
        +PlayerNetworkId: int
        +AllSynchronizables: Dictionary&ltsceneId: int, syncrhonizable: INetSynchronizable&gt
        +RegisterSynchroizable(syncrhonizable: INetSynchronizable)
        +SendRpc(sceneId: int, method: string, args: Array~object~)
        -ReceiveRpc(data: Array~object~)
        -forwardRpc(target: INetSynchronizable, method: string, latency: float, args: Array~object~)
    }

    INetSynchronizable "*" o-- NetworkSyncrhonizer: has
```

## Tool 클래스

#### 외부 인터페이스
**플레이어가 도구와 아이템 이동을 할 때**
1.  플레이어가 도구에 넘겨주는 정보 : ToolTransferArgs  
 -ItemToGive: 플레이어가 현재 손에 들고 있는 재료 아이템 (반드시 재료 아이템일 경우만 가능)
- PlayerNetworkId: 네트워크에서 플레이어를 식별할 수 있는 값 PhotonViewId등 
2. 도구에서 플레이어에게 넘겨주는 정보: ToolTransferResult
- ReceivedItem: 도구에서 작업된 아이템을 플레이어가 받게 되었을 경우 해당 아이템, 아무 아이템도 받지 못한 경우 null

**플레이어가 도구에서 작업을 할 때**
- 플레이어의 손에 아무 아이템이 없을 때만 가능
- Trigger: 실제 인터렉션이 일어나는 순간 실행해야 하는 이벤트 만약 즉시 일어나는 경우에는 Interact 후에 즉시 실행
(플레이어의 망치가 모루를 때리는 순간,  플레이어가 가지고 있는 칼날을 담금질 기름에 담그는 순간 등)
- DurationToStay: 상호작용시 플레이어 컨트롤이 중지되고 애니메이션을 재생하는 등의 시간 (초)

####  다이어그램

```mermaid
classDiagram

    class ToolTransferArgs {
        +ItemToGive: Nullable~MaterialItem~
        +PlayerNetworkId: int
    }

    class ToolTransferResult {
        +ReceivedItem: Nullable~Item~
    }

    class ToolWorkResult {
        +Trigger: Action
        +DurationToStay: float
    }

    class SmithingToolData {
        +Name: string
        +Prefab: GameObject
        +AllowedMaterialTypes: List~MaterialType~
    }

	class SmithingTool {
    	<<abstract>>
        +RemainingTime: float 
        +RemainingCount: int
        +IsFinished: bool 
        +HoldingItem: Nullable~MaterialItem~  
        +CanTransferItem(ToolTransferArgs) bool
        +Transfer(ToolTransferArgs) ToolTransferResult
        +CanWork() bool
        +Work() ToolWorkResult
        +BeforeInteract: Action~SmithingTool~
        +AfterInteract: Action~SmithingTool~
        +OnUpdate(float)
        #Data: SmithingToolData
	}

    class IInteractableTool {
        <<interface>>
        +CanTransferItem(ToolTransferArgs) bool
        +Transfer(ToolTransferArgs) ToolTransferResult
        +CanWork() bool
        +Work() ToolWorkResult
    }

    class CraftTable {
        +HasCraftableItem(Player) bool
        +ShowPrductIconUI()
        -CraftList: List~CraftData~
    }

    class Anvil {

    }

    class CraftTable {
        
    }
    
    ScriptableObject <|-- SmithingToolData: inherit
    SmithingToolData "1" o-- "1" SmithingTool: has
    IInteractableTool <|.. SmithingTool: implement
    ToolTransferArgs <-- IInteractableTool: use
    ToolTransferResult <-- IInteractableTool: use
    ToolWorkResult <-- IInteractableTool: use
    MaterialItem "0..1" o-- "1" SmithingTool: has
    MaterialType "1..*" o-- "1..*" SmithingTool: use
    SmithingTool <|-- WoodWorkTable: inherit
    SmithingTool <|-- Anvil: inherit 
    SmithingTool <|-- CraftTable: inherit
    CraftData "*" o-- "1" WoodWorkTable: has
```

## 아이템 클래스 다이어그램 (참고)

```mermaid
classDiagram
	class Item {
    	<<abstract>>
        #Data: ItemData
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
        +GetRefinedResult() MaterialItem 
        #Data: MaterialItemData
    }

    class MaterialItemData {
        +Type: MaterialType
        +RefinedResult: MaterialItemData
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
    MaterialType "1" o-- "0..*" MaterialItemData: has
    Item <|-- CompletedMaterialItem: inherit
    Item <|-- ProductItem : inherit
    CraftData "1" o-- "*" CompletedMaterialItem: has
    ICarryable <|.. Item: implement
```

