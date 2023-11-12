using System.Runtime.InteropServices;
using Api.Game.Managers;
using Api.Game.Objects;
using Api.Game.Offsets;
using Api.Game.Readers;
using Api.GameProcess;
using Api.Internal.Game.Objects;
using Api.Internal.Game.Types;
using Api.Utils;

namespace Api.Internal.Game.Managers;

[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
public struct MapNode
{
    [FieldOffset(0)] public IntPtr Child1;
    [FieldOffset(0x8)] public IntPtr Child2;
    [FieldOffset(0x10)] public IntPtr Child3;
}

[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
public struct MissileNode
{
    [FieldOffset(0)] public IntPtr Child1;
    [FieldOffset(0x8)] public IntPtr Child2;
    [FieldOffset(0x10)] public IntPtr Child3;
    [FieldOffset(0x28)] public IntPtr Missile;
}

internal class MissileManager : IMissileManager
{
    private float _listCacheDuration;
    private readonly ITargetProcess _targetProcess;
    private readonly IMissileReader _missileReader;
    private readonly IDictionary<int, IMissile> _items = new Dictionary<int, IMissile>();
    private readonly PooledList<IMissile> _itemsPool = new PooledList<IMissile>(100, 10, () => new Missile());
    private readonly uint _missileListOffset;
    
    public MissileManager(
        IBaseOffsets baseOffsets,
        ITargetProcess targetProcess,
        IMissileReader missileReader)
    {
        _targetProcess = targetProcess;
        _missileReader = missileReader;
        _missileListOffset = baseOffsets.MissileList;
    }

    public void Dispose()
    {
        _missileReader.Dispose();
    }

    private void Update(PooledList<IMissile> items)
    {
        for (var i = items.Count - 1; i >= 0; i--)
        {
            var item = items[i];
            if (_missileReader.ReadMissile(item)) continue;
            
            items.RemoveAt(i);
        }
    }

    private void CheckNode(IntPtr childPtr, HashSet<IntPtr> blockedNodes, Queue<MissileNode> nodesToVisit, HashSet<IntPtr> missilesToRead)
    {
        if (childPtr.ToInt64() < 0x1000 ||
            blockedNodes.Contains(childPtr))
        {
            return;
        }
        
        blockedNodes.Add(childPtr);
        
        if(!_targetProcess.Read<MissileNode>(childPtr, out var missileNode)) return;
        
        nodesToVisit.Enqueue(missileNode);
        if (missileNode.Missile.ToInt64() > 0x1000)
        {
            missilesToRead.Add(missileNode.Missile);
        }
    }
    
    private void FullUpdate()
    {
        _items.Clear();
        _itemsPool.Clear();
        
        if (!_targetProcess.ReadModulePointer(_missileListOffset, out var missilesPtr))
        {
            return;
        }
        
        if (!_targetProcess.Read(missilesPtr + 0x8, out missilesPtr))
        {
            return;
        }

        //Console.WriteLine("missilesPtr: " + missilesPtr.ToString("X"));
        
        var blockedNodes = new HashSet<IntPtr>();
        var missilesToRead = new HashSet<IntPtr>();
        var nodesToVisit = new Queue<MissileNode>();
        if (!_targetProcess.Read<MissileNode>(missilesPtr, out var rootNode))
        {
            return;
        }
        
        nodesToVisit.Enqueue(rootNode);
        blockedNodes.Add(missilesPtr);
        
        while (nodesToVisit.Any())
        {
            var currentNode = nodesToVisit.Dequeue();
            CheckNode(currentNode.Child1, blockedNodes, nodesToVisit, missilesToRead);
            CheckNode(currentNode.Child2, blockedNodes, nodesToVisit, missilesToRead);
            CheckNode(currentNode.Child3, blockedNodes, nodesToVisit, missilesToRead);
        }
        
        foreach (var missilePointer in missilesToRead)
        {
            var item = _itemsPool.GetNext((setItem) =>
            {
                setItem.Pointer = missilePointer;
            });
            if (!_missileReader.ReadMissile(item) || _items.ContainsKey(item.NetworkId))
            {
                _itemsPool.CancelNext();
                continue;
            }
            
            _items.Add(item.NetworkId, item);
        }
    }


    public void Update(float deltaTime)
    {
         if (_listCacheDuration < 0 && _items.Any())
         {
             Update(_itemsPool);
             _listCacheDuration += deltaTime;
         }
        else
        {
            FullUpdate();
            _listCacheDuration -= deltaTime;
        }
    }

    public IEnumerable<IMissile> GetMissiles()
    {
        return _itemsPool;
    }

    public IEnumerable<IMissile> GetMissiles(int networkId)
    {
        return GetMissiles().Where(x => x.DestinationIndex == networkId);
    }
}