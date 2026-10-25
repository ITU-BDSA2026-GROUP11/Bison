module {:extern "Bison.Razor.Models"} Models{
    class {:extern} Taxon {
        function {:extern} IsSubTaxon(ancestor: Taxon): bool
    }

    class {:extern} Observation {
        function {:extern} getTaxon(): Taxon
    }

}

module FilterByTaxon {
    import opened Models
    function FilterBy(root: Taxon, obs: seq<Observation>): seq<Observation>
    ensures forall o: Observation :: o in FilterBy(root, obs) ==> o in obs && o.getTaxon().IsSubTaxon(root)
    ensures forall o: Observation :: o in obs && o.getTaxon().IsSubTaxon(root) ==> o in FilterBy(root, obs)
    decreases |obs|
    {
    if |obs| == 0 then []
    else
    assert obs == [obs[0]] + obs[1..];
    if obs[0].getTaxon().IsSubTaxon(root) then [obs[0]] + FilterBy(root, obs[1..])
    else FilterBy(root, obs[1..])
    }
}

module FilterWrapper {
    import opened Models
    import opened FilterByTaxon

    method FilterObservations(root: Taxon, obs: seq<Observation>) returns (ObsArr: array<Observation>)
    
    ensures ObsArr[..] == FilterBy(root, obs)
    {
        var fObs := FilterBy(root, obs);
        ObsArr := new Observation[|fObs|](i => fObs[i])
    }
}


