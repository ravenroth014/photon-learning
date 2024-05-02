using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnumeratorTest : MonoBehaviour
{
    private readonly List<short> _testList = new()
    {
        4461,
        1744
    };
    
    // Start is called before the first frame update
    void Start()
    {
        var numbers = GetActiveBit();
        var enumerator = numbers.GetEnumerator();
        // newEnums = numbers.Select(number => (NewEnum)number);
        //var array = newEnums.ToArray();
        
        enumerator.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private IEnumerable<int> GetActiveBit()
    {
        for (int index = 0; index < 32; index++)
        {
            int listIndex = index / 16;
            int bitIndex = index % 16;

            int mask = 1 << bitIndex;
            int target = _testList[listIndex];
            if ((target & mask) != 0)
                yield return (listIndex * 16) + bitIndex;
        }
    }
}

public enum NewEnum : int
{
    T001 = 0,
    T002 = 1,
    T003 = 2,
    T004 = 3,
    T005 = 4,
    T006 = 5,
    T007 = 6,
    T008 = 7,
    T009 = 8,
    T010 = 9,
    T011 = 10,
    T012 = 11,
    T013 = 12,
    T014 = 13,
    T015 = 14,
    T016 = 15,
    T017 = 16,
    T018 = 17,
    T019 = 18,
    T020 = 19,
    T021 = 20,
    T022 = 21,
    T023 = 22,
    T024 = 23,
    T025 = 24,
    T026 = 25,
    T027 = 26,
    T028 = 27,
    T029 = 28,
    T030 = 29,
    T031 = 30,
    T032 = 31,
}
