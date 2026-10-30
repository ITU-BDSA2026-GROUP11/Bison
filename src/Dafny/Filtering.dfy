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
        ensures |FilterBy(root, obs)| <= |obs|
        decreases |obs|
        {
        if |obs| == 0 then []
        else
        assert obs == [obs[0]] + obs[1..];
        if obs[0].getTaxon().IsSubTaxon(root) 
        then [obs[0]] + FilterBy(root, obs[1..])
        else FilterBy(root, obs[1..])
    }
}

module FilterWrapper {
    import opened FilterByTaxon
    import opened Models

    method FilterObservations(root: Taxon, obs: seq<Observation>) returns (ObsArr: array<Observation>)
        ensures fresh(ObsArr)
        ensures forall o: Observation :: o in ObsArr[..] ==> 
            o.getTaxon().IsSubTaxon(root)
        ensures forall o: Observation :: o in ObsArr[..] && o.getTaxon().IsSubTaxon(root) ==> 
            o in ObsArr[..]
        ensures ObsArr[..] == FilterBy(root, obs)
        requires |FilterBy(root, obs)| > 0
        {
            //fObs := FilterBy(root, obs);
            //ObsArr := new Observation[|fObs|]
            //while i < |fObs|
            //ObsArr := new Observation[|FilterBy(root, obs)|];
            var fobs := FilterBy(root, obs[..]);
            ObsArr := new Observation[|fobs|](i requires 0 <= i < |fobs| => fobs[i]);
            assert ObsArr[..] == fobs;

    }
}


